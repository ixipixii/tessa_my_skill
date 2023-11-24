namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrStageInterrupter
    {
        /// <summary>
        /// Выполнить прерывания этапа.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool InterruptStage(IKrStageInterrupterContext context);
    }
}