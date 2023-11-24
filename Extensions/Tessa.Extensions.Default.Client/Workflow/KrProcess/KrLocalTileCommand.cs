using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.UI;
using Tessa.UI.Tiles;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public sealed class KrLocalTileCommand : IKrTileCommand
    {
        private readonly IKrProcessLauncher launcher;

        public KrLocalTileCommand(
            IKrProcessLauncher launcher)
        {
            this.launcher = launcher;
        }

        /// <inheritdoc />
        public async Task OnClickAsync(
            IUIContext context,
            ITile tile,
            KrTileInfo tileInfo)
        {
            var cardEditor = context?.CardEditor;
            if (tileInfo.ID == default
                || cardEditor == null)
            {
                return;
            }

            if (tileInfo.AskConfirmation
                && !TessaDialog.Confirm(LocalizationManager.Format(tileInfo.ConfirmationMessage)))
            {
                return;
            }

            using (TessaSplash.Create("$KrButton_DefaultTileSplash"))
            using (cardEditor.SetOperationInProgress(blocking: true))
            {
                if (cardEditor.CardModel.Card.StoreMode == CardStoreMode.Insert)
                {
                    await cardEditor.SaveCardAsync(context);
                    var storeResponse = context.CardEditor.LastData.StoreResponse;
                    if (storeResponse?.ValidationResult.IsSuccessful() != true)
                    {
                        return;
                    }
                }

                var process = KrProcessBuilder
                    .CreateProcess()
                    .SetProcess(tileInfo.ID)
                    .SetCard(context.CardEditor.CardModel.Card.ID)
                    .Build();

                await this.launcher.LaunchWithCardEditorAsync(process, cardEditor);
            }
        }
    }
}