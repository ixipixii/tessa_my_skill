namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    public interface IGlobalSignalHandler
    {
        IGlobalSignalHandlerResult Handle(
            IGlobalSignalHandlerContext context);
    }
}