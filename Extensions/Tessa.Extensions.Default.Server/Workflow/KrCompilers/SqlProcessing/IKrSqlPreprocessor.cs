namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing
{
    public interface IKrSqlPreprocessor
    {
        /// <summary>
        /// Выполнить предобработку запроса.
        /// </summary>
        /// <param name="context">Контекст выполнения запроса.</param>
        /// <returns>Результат предобработки запроса.</returns>
        IKrSqlPreprocessorResult Preprocess(IKrSqlExecutorContext context);
    }
}