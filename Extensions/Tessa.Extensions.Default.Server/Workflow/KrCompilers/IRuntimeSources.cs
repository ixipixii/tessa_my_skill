namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IRuntimeSources
    {
        /// <summary>
        /// Текст SQL запроса с условием. Выполняется перед выполнением этапа.
        /// Запрос должен возвращать 0 или 1.
        /// </summary>
        string RuntimeSqlCondition { get; }

        /// <summary>
        /// C# код, определяющий нужно ли выполнять группу этапов.
        /// </summary>
        string RuntimeSourceCondition { get; }

        /// <summary>
        /// C# код, выполняемый до выполнения группы этапов
        /// </summary>
        string RuntimeSourceBefore { get; }

        /// <summary>
        /// C# код, выполняемый после выполнения группы этапов.
        /// </summary>
        string RuntimeSourceAfter { get; }
    }
}