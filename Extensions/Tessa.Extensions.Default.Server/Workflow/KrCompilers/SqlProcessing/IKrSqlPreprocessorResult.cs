using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing
{
    public interface IKrSqlPreprocessorResult
    {
        /// <summary>
        /// Обработанный текст запроса.
        /// </summary>
        string Query { get; }

        /// <summary>
        /// Параметры запроса, полученные на предобработке.
        /// </summary>
        List<KeyValuePair<string, object>> Parameters { get; }

    }
}