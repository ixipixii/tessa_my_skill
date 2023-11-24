using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Actions
{
    public sealed class TaskStoreEventEmitter : CardStoreTaskExtension
    {
        private readonly IKrEventManager eventManager;

        private readonly IKrTypesCache typesCache;

        private readonly IKrScope scope;

        public TaskStoreEventEmitter(
            IKrEventManager eventManager,
            IKrTypesCache typesCache,
            IKrScope scope)
        {
            this.eventManager = eventManager;
            this.typesCache = typesCache;
            this.scope = scope;
        }

        public override Task StoreTaskBeforeRequest(
            ICardStoreTaskExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var task = context.Task;
            var cardID = context.Request.Card.ID;
            var card = new ObviousMainCardAccessStrategy(context.Request.Card);
            var validationResult = context.ValidationResult;

            if (task.State == CardRowState.Inserted)
            {
                return this.eventManager.RaiseAsync(
                    DefaultEventTypes.BeforeNewTask,
                    cardID,
                    card,
                    context,
                    validationResult,
                    cancellationToken: context.CancellationToken);
            }

            if (task.OptionID.HasValue && task.Action == CardTaskAction.Complete)
            {
                return this.eventManager.RaiseAsync(
                    DefaultEventTypes.BeforeCompleteTask,
                    cardID,
                    card,
                    context,
                    validationResult,
                    cancellationToken: context.CancellationToken);
            }

            return Task.CompletedTask;
        }

        public override Task StoreTaskBeforeCommitTransaction(
            ICardStoreTaskExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var task = context.Task;
            var cardID = context.Request.Card.ID;
            var card = new KrScopeMainCardAccessStrategy(cardID, this.scope);
            var validationResult = context.ValidationResult;

            if (task.State == CardRowState.Inserted)
            {
                return this.eventManager.RaiseAsync(DefaultEventTypes.NewTask, cardID, card, context, validationResult, cancellationToken: context.CancellationToken);
            }

            if (task.OptionID.HasValue
                && task.Action == CardTaskAction.Complete)
            {
                return this.eventManager.RaiseAsync(DefaultEventTypes.CompleteTask, cardID, card, context, validationResult, cancellationToken: context.CancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}