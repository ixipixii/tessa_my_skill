
namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    /// <summary>
    /// Интерфейс скриптов элемента процесса.
    /// Под элементом процесса понимается группа, шаблон и этап в равной степени.
    /// </summary>
    public interface IKrProcessItemScript
    {
        /// <summary>
        /// Запуск скрипта инициализации.
        /// </summary>
        void RunBefore();

        /// <summary>
        /// Скрипт инициализации.
        /// </summary>
        void Before();

        /// <summary>
        /// Запуск вычисления условия. 
        /// </summary>
        /// <returns>
        /// Признак того, что единица исполнения подтверждена.
        /// </returns>
        bool RunCondition();

        /// <summary>
        /// Вычисление условия.
        /// </summary>
        /// <returns>Признак того, что единица исполнения подтверждена.</returns>
        bool Condition();

        /// <summary>
        /// Запуск скрипта постобработки.
        /// </summary>
        void RunAfter();
        
        /// <summary>
        /// Скрипт постобработки.
        /// </summary>
        void After();
    }
}