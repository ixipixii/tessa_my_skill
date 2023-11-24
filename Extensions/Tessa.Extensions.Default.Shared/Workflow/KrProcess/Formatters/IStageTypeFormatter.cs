namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public interface IStageTypeFormatter
    {
        void FormatClient(IStageTypeFormatterContext context);

        void FormatServer(IStageTypeFormatterContext context);
    }
}