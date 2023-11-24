using System;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrAsyncProcessRunner : KrProcessRunnerBase
    {
        public KrAsyncProcessRunner(
            IKrProcessContainer container,
            IKrCompilationCache compilationCache,
            [Dependency(KrExecutorNames.CacheExecutor)] Func<IKrExecutor> executorFunc,
            IKrScope scope,
            Func<IKrSqlPreprocessor> sqlPreprocessorFunc,
            IDbScope dbScope,
            IKrProcessCache processCache,
            IUnityContainer unityContainer,
            ISession session,
            IKrProcessRunnerProvider runnerProvider,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryEwt,
            IKrTokenProvider tokenProvider,
            IKrTypesCache typesCache,
            ICardMetadata cardMetadata,
            IKrProcessStateMachine stateMachine,
            IKrStageInterrupter interrupter,
            IKrSqlExecutor sqlExecutor,
            ICardCache cardCache,
            IKrStageSerializer serializer,
            IObjectModelMapper mapper)
            : base(
                container, 
                compilationCache,
                executorFunc,
                scope, 
                dbScope,
                processCache, 
                unityContainer,
                session,
                runnerProvider,
                typesCache,
                cardMetadata,
                stateMachine,
                interrupter,
                sqlExecutor,
                cardCache, 
                serializer,
                mapper)
        {
        }

        /// <inheritdoc />
        protected override KrProcessRunnerMode RunnerMode { get; } = KrProcessRunnerMode.Async;

        protected override bool Prepare(IKrProcessRunnerContext context)
        {
            if (this.Scope.HasLaunchedRunner(context.ProcessInfo.ProcessID))
            {
                context.ValidationResult.AddError(this, "$KrProcess_ErrorMessage_NestedProcessRunner");
                return false;
            }
            this.Scope.AddLaunchedRunner(context.ProcessInfo.ProcessID);
            
            if (context.InitiationCause != KrProcessRunnerInitiationCause.StartProcess)
            {
                return true;
            }

            if (context.WorkflowProcess.CurrentApprovalStageRowID.HasValue)
            {
                context.ValidationResult.AddError(this, "$KrStages_ProcessAlreadyStarted");
                return false;
            }

            this.InitialRecalc(context);
            if (!context.ValidationResult.IsSuccessful())
            {
                return false;
            }

            if (context.WorkflowProcess.Stages.Count == 0)
            {
                context.ValidationResult.AddError(this, KrErrorHelper.FormatEmptyRoute(context.SecondaryProcess));
                return false;
            }

            foreach (var stage in context.WorkflowProcess.Stages)
            {
                stage.State = KrStageState.Inactive;
            }

            if (context.ProcessInfo?.ProcessTypeName == KrConstants.KrProcessName
                && context.WorkflowProcess.Author is null)
            {
                this.SetAuthor(context);
            }

            return true;
        }

        protected override void Finalize(
            IKrProcessRunnerContext context,
            Exception exc = null)
        {
            if (context.WorkflowProcess.Stages.Count != 0
                && context.InitiationCause == KrProcessRunnerInitiationCause.StartProcess
                && context.WorkflowProcess.Stages.All(p => p.State == KrStageState.Skipped || p.Hidden))
            {
                context.ValidationResult.AddError(this, KrErrorHelper.FormatEmptyRoute(context.SecondaryProcess));
            }
            this.Scope.RemoveLaunchedRunner(context.ProcessInfo.ProcessID);
        }

        protected override NextAction ProcessStageHandlerResult(
            Stage stage,
            StageHandlerResult result,
            IKrProcessRunnerContext context)
        {
            // InProgress и None не делают ничего.
            if (result.Action != StageHandlerAction.InProgress
                && result.Action != StageHandlerAction.None)
            {
                return base.ProcessStageHandlerResult(stage, result, context);
            }

            return new NextAction();
        }

        private void InitialRecalc(
            IKrProcessRunnerContext context)
        {
            if (!context.CardID.HasValue)
            {
                return;
            }

            var executionUnits = context.SecondaryProcess != null
                ? this.ProcessCache.GetStageGroupsForSecondaryProcess(context.SecondaryProcess.ID).Select(p => p.ID)
                : null;
            var cardLoadingStrategy = new KrScopeMainCardAccessStrategy(context.CardID.Value, this.Scope);
            var ctx = new KrExecutionContext(
                context.CardContext,
                cardLoadingStrategy,
                context.CardID,
                context.CardTypeID,
                context.CardTypeName,
                context.CardTypeCaption,
                context.DocTypeID,
                context.KrComponents,
                context.WorkflowProcess,
                compilationResult: null,
                executionUnits: executionUnits, // или null, тогда выполнится все что возможно
                secondaryProcess: context.SecondaryProcess // или null
            );

            var executor = this.ExecutorFunc();
            var result = executor.Execute(ctx);
            context.ValidationResult.Add(result.Result);
        }

        private void SetAuthor(
            IKrProcessRunnerContext context)
        {
            var process = context.WorkflowProcess;
            var contextualSatellite = context.ContextualSatellite;
            var user = this.Session.User;

            // Для дальнейшей доступности автора в объектной модели
            process.Author = new Author(user.ID, user.Name);
            
            // Для дальнейшей доустпности в холдере.
            context.ProcessHolder.PrimaryProcessCommonInfo.AuthorID = user.ID;
            context.ProcessHolder.PrimaryProcessCommonInfo.AuthorName = user.Name;
            
            // Для дальнейшей доступности автора в сателлите
            var aciFields = contextualSatellite.GetApprovalInfoSection().RawFields;
            aciFields[KrConstants.KrApprovalCommonInfo.AuthorID] = user.ID;
            aciFields[KrConstants.KrApprovalCommonInfo.AuthorName] = user.Name;

            // Внесем изменения в базу для доступа из контекстных ролей
            var db = this.DbScope.Db;
            db.SetCommand(
                    this.DbScope.BuilderFactory
                        .Update(KrConstants.KrApprovalCommonInfo.Name)
                        .C("AuthorID").Assign().P("aid")
                        .C("AuthorName").Assign().P("an")
                        .Where().C("ID").Equals().P("ID")
                        .Build(),
                    db.Parameter("aid", user.ID),
                    db.Parameter("an", user.Name),
                    db.Parameter("ID", contextualSatellite.ID))
                .LogCommand()
                .ExecuteNonQuery();
        }
    }
}
