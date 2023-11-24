namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrSecondaryProcessExecutionEvaluator
    {
        bool Evaluate(
            IKrSecondaryProcessEvaluatorContext context);
    }
}