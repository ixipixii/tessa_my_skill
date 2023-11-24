using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public interface IStageTypeUIHandler
    {
        /// <summary>
        /// Выполняется при открытии редактирования строки.
        /// </summary>
        /// <param name="context">Контекст расширений.</param>
        /// <returns>Асинхронная задача.</returns>
        Task Initialize(
            IKrStageTypeUIHandlerContext context);

        /// <summary>
        /// Выполняется валидация.
        /// </summary>
        /// <param name="context">Контекст расширений.</param>
        /// <returns>Асинхронная задача.</returns>
        Task Validate(
            IKrStageTypeUIHandlerContext context);

        /// <summary>
        /// Выполняется при закрытии строки.
        /// </summary>
        /// <param name="context">Контекст расширений.</param>
        /// <returns>Асинхронная задача.</returns>
        Task Finalize(
            IKrStageTypeUIHandlerContext context);
    }
}