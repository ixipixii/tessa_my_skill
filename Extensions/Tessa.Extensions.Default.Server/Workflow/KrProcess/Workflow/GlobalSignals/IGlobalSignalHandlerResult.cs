namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    /// <summary>
    /// Описывает результат обработки глобального сигнала.
    /// </summary>
    public interface IGlobalSignalHandlerResult
    {
        /// <summary>
        /// Возвращает признак обработки сигнала.
        /// </summary>
        bool Handled { get; }
        
        /// <summary>
        /// После обработки состояния можно продолжать текущее выполнение runner-a.
        /// Если <c>false</c>, то текущее выполнение будет прервано.
        /// Если запланировано еще одно сохранение карточки, приводящее к запуску runner-a, то runner будет запущен с текущим KrScope.
        /// </summary>
        bool ContinueCurrentRun { get; }
        
        /// <summary>
        /// Явно выполнить запуск новой группы, несмотря на то, что у старого и нового этапа остались.
        /// Если 
        /// </summary>
        bool ForceStartGroup { get; }
        
    }
}