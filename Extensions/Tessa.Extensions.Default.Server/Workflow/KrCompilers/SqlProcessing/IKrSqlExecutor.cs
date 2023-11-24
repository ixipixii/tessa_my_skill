using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing
{
    public interface IKrSqlExecutor
    {
        /// <summary>
        /// Вычисление условного SQL-выражения.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool ExecuteCondition(IKrSqlExecutorContext context);

        /// <summary>
        /// Вычисление списка SQL-исполнителей.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        List<Performer> ExecutePerformers(IKrSqlExecutorContext context);

    }
}