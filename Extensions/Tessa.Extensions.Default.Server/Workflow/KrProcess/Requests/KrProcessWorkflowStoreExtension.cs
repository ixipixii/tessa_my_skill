using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Settings;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrProcessWorkflowStoreExtension : KrWorkflowStoreExtension
    {
        #region fields

        private readonly IKrTypesCache typesCache;
        private readonly KrSettingsLazy settingsLazy;
        private readonly IKrProcessRunner asyncRunner;
        private readonly IObjectModelMapper objectModelMapper;
        private readonly IKrScope krScope;
        private readonly IKrTokenProvider tokenProvider;
        private readonly IKrProcessContainer processContainer;
        private readonly IKrProcessCache processCache;
        private readonly IKrEventManager eventManager;

        #endregion

        #region constructor

        public KrProcessWorkflowStoreExtension(
            IKrTokenProvider krTokenProvider,
            [Dependency(CardRepositoryNames.DefaultWithoutTransaction)] ICardRepository cardRepositoryToCreateNextRequest,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryToStoreNextRequest,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryToCreateTasks,
            ICardTaskHistoryManager taskHistoryManager,
            ICardGetStrategy cardGetStrategy,
            IWorkflowQueueProcessor workflowQueueProcessor,
            IKrTypesCache typesCache,
            KrSettingsLazy settingsLazy,
            IKrProcessRunnerProvider runnerProvider,
            IObjectModelMapper objectModelMapper,
            IKrScope krScope,
            IKrTokenProvider tokenProvider,
            IKrProcessContainer processContainer,
            IKrProcessCache processCache,
            IKrEventManager eventManager)
            : base(
                  krTokenProvider,
                  cardRepositoryToCreateNextRequest,
                  cardRepositoryToStoreNextRequest,
                  cardRepositoryToCreateTasks,
                  taskHistoryManager,
                  cardGetStrategy,
                  workflowQueueProcessor)
        {
            this.typesCache = typesCache ?? throw new ArgumentNullException(nameof(typesCache));
            this.settingsLazy = settingsLazy ?? throw new NullReferenceException(nameof(settingsLazy));
            this.asyncRunner = runnerProvider.GetRunner(KrProcessRunnerNames.Async);
            this.objectModelMapper = objectModelMapper;
            this.krScope = krScope;
            this.tokenProvider = tokenProvider;
            this.processContainer = processContainer;
            this.processCache = processCache;
            this.eventManager = eventManager;
        }

        #endregion

        #region base overrides

        /// <inheritdoc />
        public override Task AfterRequest(
            ICardStoreExtensionContext context)
        {
            if (context.Info.GetAsyncProcessCompletedSimultaniosly())
            {
                context.Response.Info.SetAsyncProcessCompletedSimultaniosly();
            }

            var pi = context.Info.GetProcessInfoAtEnd();
            if (pi != null)
            {
                context.Response.Info.SetProcessInfoAtEnd(pi);
            }

            return Task.CompletedTask;
        }

        protected override async ValueTask<bool> CardIsAllowedAsync(
            Card card,
            ICardStoreExtensionContext context) => true;

        protected override async ValueTask<bool> TaskIsAllowedAsync(CardTask task, ICardStoreExtensionContext context) =>
            this.processContainer.IsTaskTypeRegistered(task.TypeID);

        protected override async ValueTask<bool> CanHandleQueueItemAsync(WorkflowQueueItem queueItem, ICardStoreExtensionContext context) =>
            KrConstants.KrProcessName == queueItem.Signal.ProcessTypeName
            || KrConstants.KrSecondaryProcessName == queueItem.Signal.ProcessTypeName
            || KrConstants.KrNestedProcessName == queueItem.Signal.ProcessTypeName;

        protected override async ValueTask<bool> CanStartProcessAsync(
            Guid? processID,
            string processName,
            ICardStoreExtensionContext context)
        {
            if (KrConstants.KrProcessName != processName
                && KrConstants.KrSecondaryProcessName != processName
                && KrConstants.KrNestedProcessName != processName)
            {
                return false;
            }

            var card = context.Request.Card;
            var allowed = KrProcessHelper
                .CardSupportsRoutes(card, context.DbScope, this.typesCache);
            if (!allowed)
            {
                context.ValidationResult.AddError(this, "$KrProcess_Disabled");
            }
            return allowed;
        }

        protected override Task StartProcessAsync(
            Guid? processID,
            string processName,
            IWorkflowWorker workflowWorker,
            CancellationToken cancellationToken = default)
        {
            if (processName == KrConstants.KrSecondaryProcessName)
            {
                // Для вторичного процесса нужно создать сателлит.
                var manager = (KrProcessWorkflowManager) workflowWorker.Manager;
                if (manager.WorkflowContext.Request.TryGetStartingProcessName() != processName)
                {
                    return Task.CompletedTask;
                }

                processID = processID ?? Guid.NewGuid();
                var card = this.krScope.CreateSecondaryKrSatellite(manager.WorkflowContext.CardID, processID.Value);
                manager.SpecifySatelliteID(card.ID);
            }
            return workflowWorker.StartProcessAsync(processName, newProcessID: processID, cancellationToken: cancellationToken);
        }

        protected override async ValueTask<IWorkflowContext> CreateContextAsync(
            ICardStoreExtensionContext context,
            CardStoreRequest nextRequest)
        {
            this.MoveTokenToNextRequest(context.Request.Card, nextRequest);
            var card = context.Request.Card;
            var docTypeID = KrProcessSharedHelper.GetDocTypeID(card, context.DbScope);
            var components = KrComponentsHelper.GetKrComponents(card.TypeID, docTypeID, this.typesCache);
            return new KrProcessWorkflowContext(
                card.ID,
                card.TypeID,
                card.TypeName,
                card.TypeCaption,
                docTypeID,
                components,
                context,
                nextRequest,
                this.CardRepositoryToCreateTasks,
                this.TaskHistoryManager,
                this.CardGetStrategy,
                this.objectModelMapper,
                this.asyncRunner,
                this.krScope,
                this.settingsLazy,
                this.processCache,
                this.eventManager);
        }

        protected override async ValueTask<IWorkflowManager> CreateManagerAsync(
            IWorkflowContext workflowContext,
            CancellationToken cancellationToken = default) =>
            new KrProcessWorkflowManager((KrProcessWorkflowContext)workflowContext, this.WorkflowQueueProcessor);

        protected override async ValueTask<IWorkflowWorker> CreateWorkerAsync(
            IWorkflowManager workflowManager,
            CancellationToken cancellationToken = default) =>
            new KrProcessWorkflowWorker((KrProcessWorkflowManager)workflowManager);

        #endregion

        #region private

        private void MoveTokenToNextRequest(Card card, CardStoreRequest nextRequest)
        {
            // если карточка использует права доступа, рассчитываемые через KrToken,
            // то мы можем вычислить права на сохранение по Workflow по правам на предыдущее сохранение
            KrToken krToken = KrToken.TryGet(card.Info);
            if (krToken != null)
            {
                Card nextCard = nextRequest.Card;
                KrToken nextKrToken = this.tokenProvider.CreateToken(nextCard, krToken.PermissionsVersion, krToken.Permissions);

                nextKrToken.Set(nextCard.Info);
            }

            // на крайний случай запрещаем кидать предупреждения при нерассчитанных правах
            // на сохранение по Workflow; тогда права рассчитываются автоматически
            nextRequest.SetIgnorePermissionsWarning();
        }

        #endregion
    }
}