namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IExecutionSources
    {
        /// <summary>
        /// Текст SQL запроса с условием. Выполняется для определения доступности для исполнения.
        /// </summary>
        string ExecutionSqlCondition { get; }

        /// <summary>
        /// C# код, определяющий доступность для исполнения.
        /// </summary>
        string ExecutionSourceCondition { get; }
    }
}