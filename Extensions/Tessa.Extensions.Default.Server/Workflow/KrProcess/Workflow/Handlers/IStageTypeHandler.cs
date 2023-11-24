namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    /// <summary>
    /// Описывает обработчик этапа.
    /// </summary>
    public interface IStageTypeHandler
    {
        /// <summary>
        /// Метод, вызываемый перед вызовом предскрипта этапа.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        void BeforeInitialization(
            IStageTypeHandlerContext context);

        /// <summary>
        /// Метод, вызываемый после вызова постскрипта этапа.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        void AfterPostprocessing(
            IStageTypeHandlerContext context);
        
        /// <summary>
        /// Обработка старта этапа. 
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns>Результат выполнения этапа.</returns>
        StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context);

        /// <summary>
        /// Обработка завершения задания.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns>Результат выполнения этапа.</returns>
        StageHandlerResult HandleTaskCompletion(
            IStageTypeHandlerContext context);

        /// <summary>
        /// Обработка возврата задания на роль.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns>Результат выполнения этапа.</returns>
        StageHandlerResult HandleTaskReinstate(
            IStageTypeHandlerContext context);

        /// <summary>
        /// Обработка сигнала.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns>Результат выполнения этапа.</returns>
        StageHandlerResult HandleSignal(
            IStageTypeHandlerContext context);

        /// <summary>
        /// Обработка восстановления процесса.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns>Результат выполнения этапа.</returns>
        /// <returns></returns>
        StageHandlerResult HandleResurrection(
            IStageTypeHandlerContext context);
        
        /// <summary>
        /// Обработка отмены этапа.
        /// Данный метод должен утилизировать все используемые этапом ресурсы.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа</param>
        /// <returns> Удалось полностью прервать выполнение
        /// <c>true</c> Этап прерван и процесс идет дальше
        /// <c>false</c> Этап не завершился до конца и метод необходимо вызвать в NextRequest повторно,
        /// т.к. сейчас в текущем сохранении невозможно утилизировать все используемые ресурсы.</returns>
        bool HandleStageInterrupt(
            IStageTypeHandlerContext context);
    }
}