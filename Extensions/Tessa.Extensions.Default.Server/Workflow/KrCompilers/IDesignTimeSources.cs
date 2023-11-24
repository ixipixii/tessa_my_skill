namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IDesignTimeSources
    {
        /// <summary>
        /// Текст SQL запроса с условием. Выполняется при расчете.
        /// Запрос должен возвращать 0 или 1.
        /// При одновременном выполнении Sql-условия и условия C# из SourceCondition
        /// группа этапов считается подтвержденной (Confirmed). 
        /// </summary>
        string SqlCondition { get; }

        /// <summary>
        /// C# код, выполняемый до кода SourceCondition
        /// При одновременном выполнении Sql-условия и условия C# из SourceCondition
        /// шаблон считается подтвержденным (Confirmed). 
        /// </summary>
        string SourceCondition { get; }

        /// <summary>
        /// C# код, выполняемый до кода SourceCondition
        /// </summary>
        string SourceBefore { get; }

        /// <summary>
        /// C# код, выполняемый после SourceCondition и подстановки данных в карточку
        /// </summary>
        string SourceAfter { get; }
    }
}