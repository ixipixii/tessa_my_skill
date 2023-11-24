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
    public sealed class KrSyncProcessRunner : KrProcessRunnerBase
    {
        public KrSyncProcessRunner(
            IKrProcessContainer container,
            IKrCompilationCache compilationCache,
            [Dependency(KrExecutorNames.CacheExecutor)] Func<IKrExecutor> executorFunc,
            IKrScope scope,
            IDbScope dbScope,
            IKrProcessCache processCache,
            IUnityContainer unityContainer,
            ISession session,
            IKrProcessRunnerProvider runnerProvider,
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
        protected override KrProcessRunnerMode RunnerMode { get; } = KrProcessRunnerMode.Sync;

        protected override bool Prepare(IKrProcessRunnerContext context)
        {
            if (context.InitiationCause != KrProcessRunnerInitiationCause.InMemoryLaunching
                && context.InitiationCause != KrProcessRunnerInitiationCause.Resurrection)
            {
                context.ValidationResult.AddError(this, $"{this.GetType().Name} works only with" +
                    $" {nameof(KrProcessRunnerInitiationCause)}.{nameof(KrProcessRunnerInitiationCause.InMemoryLaunching)}");
                return false;
            }

            if (context.WorkflowProcess.Stages.Count == 0)
            {
                context.ValidationResult.AddError(this, KrErrorHelper.FormatEmptyRoute(context.SecondaryProcess));
                return false;
            }

            return true;
        }

        protected override void Finalize(
            IKrProcessRunnerContext context,
            Exception exc = null)
        {
            if (context.WorkflowProcess.Stages.Count != 0
                && context.WorkflowProcess.Stages.All(p => p.State == KrStageState.Skipped))
            {
                context.ValidationResult.AddError(this, KrErrorHelper.FormatEmptyRoute(context.SecondaryProcess));
            }
        }

        protected override NextAction ProcessStageHandlerResult(
            Stage stage,
            StageHandlerResult result,
            IKrProcessRunnerContext context)
        {
            if (result.Action != StageHandlerAction.InProgress)
            {
                return base.ProcessStageHandlerResult(stage, result, context);
            }

            context.ValidationResult.AddError(this,
                $"{this.GetType().Name} can't handle " +
                $"{nameof(StageHandlerAction)}.{nameof(StageHandlerAction.InProgress)}");
            return new NextAction();
        }

    }
}