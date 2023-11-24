using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.UI;
using Tessa.UI.Cards;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public static class KrProcessClientLauncherExtensions
    {
        public static IKrProcessLaunchResult LaunchWithCardEditor(
            this IKrProcessLauncher launcher,
            KrProcessInstance krProcess,
            ICardEditorModel cardEditor)
        {
            using (UIContext.Create(new UIContext(cardEditor)))
            {
                return launcher.Launch(krProcess);
            }
        }

        public static async Task<IKrProcessLaunchResult> LaunchWithCardEditorAsync(
            this IKrProcessLauncher launcher,
            KrProcessInstance krProcess,
            ICardEditorModel cardEditor)
        {
            krProcess = KrProcessBuilder
                .ModifyProcess(krProcess)
                .Build();

            await using (UIContext.Create(new UIContext(cardEditor)))
            {
                var specificParam = new KrProcessClientLauncher.SpecificParameters { UseCurrentCardEditor = true };
                return await launcher.LaunchAsync(krProcess, null, specificParam);
            }
        }
    }
}