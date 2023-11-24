namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public interface IStateHandler
    {
        IStateHandlerResult Handle(
            IStateHandlerContext context);
    }
}