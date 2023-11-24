
namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Кэш последней сборки сгенерированного кода
    /// </summary>
    public interface IKrCompilationCache
    {
        /// <summary>
        /// Выполняет сборку, если в кэше нет сборки или сборка invalidate
        /// Если в кэше есть актуальная сборка, возвращается null
        /// </summary>
        /// <returns>Результат сборки или null</returns>
        IKrCompilationResult Build();

        /// <summary>
        /// Явно сбрасывается кэш сборки и выполняется пересборка
        /// Кэш KrStageTenolateCache не сбрасывается
        /// </summary>
        /// <returns>Результат компиляции</returns>
        IKrCompilationResult Rebuild();

        /// <summary>
        /// Получение значения из кэша
        /// </summary>
        /// <returns>Результат сборки</returns>
        IKrCompilationResult Get();

        /// <summary>
        /// Сброс значения кэша
        /// </summary>
        void Invalidate();
    }
}
