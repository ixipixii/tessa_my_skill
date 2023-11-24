using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public sealed class InterruptionStateHandler : StateHandlerBase
    {
        private readonly IKrScope krScope;
        private readonly IKrStageInterrupter interrupter;

        public InterruptionStateHandler(
            IKrScope krScope,
            IKrStageInterrupter interrupter)
        {
            this.krScope = krScope;
            this.interrupter = interrupter;
        }

        /// <inheritdoc />
        public override IStateHandlerResult Handle(
            IStateHandlerContext context)
        {
            var state = context.State;
            var directionAfterInterrupt =
                (DirectionAfterInterrupt) state.Parameters.TryGet<int>(KrConstants.Keys.DirectionAfterInterruptParam);
            var completelyInterrupted = this.interrupter.InterruptStage(new KrStageInterrupterContext(
                directionAfterInterrupt,
                context.Stage,
                context.RunnerContext,
                context.RunnerMode,
                ci => ci ? state.NextState : state));

            if (!completelyInterrupted)
            {
                return StateHandlerResult.WithoutContinuationProcessResult;
            }
            return this.krScope.IsDefaultProcessState(context.RunnerContext.ProcessInfo.ProcessID) 
                ? StateHandlerResult.EmptyResult
                : StateHandlerResult.ContinueCurrentRunResult;
        }
    }
}