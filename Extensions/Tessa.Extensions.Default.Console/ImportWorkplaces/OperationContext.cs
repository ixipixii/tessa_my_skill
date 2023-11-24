namespace Tessa.Extensions.Default.Console.ImportWorkplaces
{
    /// <summary>
    ///     Контекст операции импорта рабочих мест
    /// </summary>
    public class OperationContext
    {
        /// <summary>
        ///     Gets or sets a value indicating whether Признак импорта ролей
        /// </summary>
        public bool ImportRoles { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Признак импорта внедренных поисковых запросов
        /// </summary>
        public bool ImportSearchQueries { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Признак импорта внедренных представлений
        /// </summary>
        public bool ImportViews { get; set; }

        /// <summary>
        ///     Gets or sets Источник файлов
        /// </summary>
        public string Source { get; set; }
    }
}