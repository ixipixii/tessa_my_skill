namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public interface IKrProcessExecutionScript
    {

        /// <summary>
        /// Запуск определения условия выполнимости процесса.
        /// </summary>
        bool RunExecution();

        /// <summary>
        /// Определения условия выполнимости процесса.
        /// </summary>
        bool Execution();
    }
}