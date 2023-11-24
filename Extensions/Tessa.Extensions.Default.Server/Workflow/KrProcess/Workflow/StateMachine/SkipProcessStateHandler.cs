
namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public sealed class SkipProcessStateHandler : StateHandlerBase
    {
        /// <inheritdoc />
        public override IStateHandlerResult Handle(
            IStateHandlerContext context)
        {
            var workflowProcess = context.RunnerContext.WorkflowProcess;
            var currentStage = context.Stage;
            context.RunnerContext.WorkflowProcess.CurrentApprovalStageRowID = null;
            context.RunnerContext.PreparingGroupStrategy = new DisableRecalcPreparingGroupRecalcStrategy();
            TransitionHelper.SetSkipStateToSubsequentStages(
                currentStage,
                workflowProcess.Stages,
                context.RunnerContext.ProcessHolderSatellite);

            return StateHandlerResult.WithoutContinuationProcessResult;
        }
    }
}