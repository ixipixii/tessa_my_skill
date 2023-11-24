using System.Threading.Tasks;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public interface IKrProcessLauncher
    {
        /// <summary>
        /// Запуск kr-процесса.
        /// </summary>
        /// <param name="krProcess"></param>
        /// <param name="cardContext"></param>
        /// <param name="specificParameters">Специфичные для реализации параметры запуска</param>
        /// <returns></returns>
        IKrProcessLaunchResult Launch(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null);

        /// <summary>
        /// Запуск kr-процесса.
        /// </summary>
        /// <param name="krProcess"></param>
        /// <param name="cardContext"></param>
        /// <param name="specificParameters">Специфичные для реализации параметры запуска</param>
        /// <returns></returns>
        Task<IKrProcessLaunchResult> LaunchAsync(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null);
    }
}