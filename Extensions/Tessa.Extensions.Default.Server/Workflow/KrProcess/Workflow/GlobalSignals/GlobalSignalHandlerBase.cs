namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    public abstract class GlobalSignalHandlerBase: IGlobalSignalHandler
    {
        /// <inheritdoc />
        public virtual IGlobalSignalHandlerResult Handle(
            IGlobalSignalHandlerContext context)
        {
            return GlobalSignalHandlerResult.EmptyHandlerResult;
        }
    }
}