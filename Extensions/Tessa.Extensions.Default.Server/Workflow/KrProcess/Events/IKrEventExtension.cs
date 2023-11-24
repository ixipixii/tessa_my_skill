using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    /// <summary>
    /// Расширение на событие KrProcess.
    /// </summary>
    public interface IKrEventExtension : IExtension
    {
        /// <summary>
        /// Обработка события KrProcess
        /// </summary>
        /// <param name="context">Контекст расширений.</param>
        /// <returns>Асинхронная задача.</returns>
        Task HandleEvent(IKrEventExtensionContext context);
    }
}