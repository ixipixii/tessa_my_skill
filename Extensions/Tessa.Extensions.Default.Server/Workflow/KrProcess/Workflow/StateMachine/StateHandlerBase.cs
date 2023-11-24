namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public abstract class StateHandlerBase: IStateHandler
    {
        /// <inheritdoc />
        public virtual IStateHandlerResult Handle(
            IStateHandlerContext context) => StateHandlerResult.EmptyResult;
    }
}