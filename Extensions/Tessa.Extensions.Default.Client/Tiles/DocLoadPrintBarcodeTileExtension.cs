using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Platform.Client.Tiles;
using Tessa.Platform;
using Tessa.Platform.IO;
using Tessa.Platform.Licensing;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Default.Client.Tiles
{
    public sealed class DocLoadPrintBarcodeTileExtension : TileExtension
    {
        #region Constructors

        public DocLoadPrintBarcodeTileExtension(ICardStreamClientRepository cardStreamClientRepository,
            ICardRepository cardRepository,
            PrintDialogProvider printProvider,
            ICardCache cardCache,
            ILicenseManager licenseManager)
        {
            this.cardStreamClientRepository = cardStreamClientRepository;
            this.cardRepository = cardRepository;
            this.printProvider = printProvider;
            this.cardCache = cardCache;
            this.licenseManager = licenseManager;
        }

        #endregion

        #region Fields

        private readonly ICardStreamClientRepository cardStreamClientRepository;
        private readonly ICardRepository cardRepository;
        private readonly PrintDialogProvider printProvider;
        private readonly ICardCache cardCache;
        private readonly ILicenseManager licenseManager;

        #endregion

        #region Private Methods

        private async void EvaluatingPrintBarcode(object sender, TileEvaluationEventArgs e)
        {
            ICardEditorModel editor = e.CurrentTile.Context.CardEditor;

            using (e.Defer())
            {
                Card settingsCard = await cardCache.Cards.GetAsync("DocLoad").ConfigureAwait(false);

                IDictionary<string, object> fields = settingsCard.Sections["DocLoadSettings"].Fields;
                var isEnabled = fields.TryGet<bool>("IsEnabled");
                var tableName = fields.TryGet<string>("DefaultBarcodeTableName");
                var fieldName = fields.TryGet<string>("DefaultBarcodeFieldName");

                ICardModel model;
                e.SetIsEnabledWithCollapsing(
                    e.CurrentTile,
                    isEnabled
                    && editor != null
                    && (model = editor.CardModel) != null
                    && model.Card.StoreMode == CardStoreMode.Update
                    && model.Card.Sections.ContainsKey(tableName)
                    && model.Card.Sections[tableName].Fields.ContainsKey(fieldName));
            }
        }

        private async void EvaluatingSelectPrinter(object sender, TileEvaluationEventArgs e)
        {
            ICardEditorModel editor = e.CurrentTile.Context.CardEditor;

            using (e.Defer())
            {
                Card settingsCard = await cardCache.Cards.GetAsync("DocLoad").ConfigureAwait(false);

                IDictionary<string, object> fields = settingsCard.Sections["DocLoadSettings"].Fields;
                var isEnabled = fields.TryGet<bool>("IsEnabled");
                var tableName = fields.TryGet<string>("DefaultBarcodeTableName");
                var fieldName = fields.TryGet<string>("DefaultBarcodeFieldName");

                ICardModel model;
                e.SetIsEnabledWithCollapsing(
                    e.CurrentTile,
                    isEnabled
                    && editor != null
                    && (model = editor.CardModel) != null
                    && model.Card.Sections.ContainsKey(tableName)
                    && model.Card.Sections[tableName].Fields.ContainsKey(fieldName)
                    && printProvider.IsPrinterSelectionEnabled());
            }
        }

        private async Task<byte[]> DownloadBarcode(Guid? cardId)
        {
            IUIContext context = UIContext.Current;
            ICardEditorModel editor = context.CardEditor;
            ICardModel model = editor?.CardModel;

            if (model == null)
            {
                return null;
            }

            var request = new CardGetFileContentRequest
            {
                CardID = cardId,
                FileID = Guid.Empty,
                FileName = "Barcode.bmp",
                VersionRowID = Guid.Empty,
                FileTypeName = DefaultFileTypes.Barcode
            };

            request.SetDigest(model.Digest);

            using (TessaSplash.Create(TessaSplashMessage.OpeningFile))
            {
                byte[] content = null;

                CardGetFileContentResponse getFileContentResponse =
                    await this.cardStreamClientRepository
                        .GetFileContentAsync(
                            request,
                            async (contentStream, ct) =>
                                content = await contentStream.ReadAllBytesAsync(ct).ConfigureAwait(false));

                ValidationResult result = getFileContentResponse.ValidationResult.Build();
                TessaDialog.ShowNotEmpty(result);

                if (!result.IsSuccessful)
                {
                    return null;
                }

                if (getFileContentResponse.Info.TryGet<bool>("RefreshCard"))
                {
                    if (model.Card.StoreMode == CardStoreMode.Insert || await model.HasChangesAsync())
                    {
                        if (!TessaDialog.Confirm("$UI_Common_ConfirmSave"))
                        {
                            return null;
                        }
                    }

                    if (!await editor.SaveCardAsync(context))
                    {
                        return null;
                    }
                }

                return getFileContentResponse.HasContent ? content : null;
            }
        }

        #endregion

        #region Command Actions

        private void PrintAction(object parameter)
        {
            this.SelectPrinterAndPrintAsync(false);
        }

        private async void SelectPrinterAndPrintAsync(bool forceSelect)
        {
            IUIContext context = UIContext.Current;
            ICardEditorModel editor = context.CardEditor;
            var model = editor?.CardModel;
            if (model == null)
            {
                return;
            }

            Card card = model.Card;

            byte[] barcodeBytes = await this.DownloadBarcode(card.ID).ConfigureAwait(false);
            if (barcodeBytes == null)
            {
                return;
            }

            if (this.printProvider.SelectPrinterDialog(forceSelect) != true)
            {
                return;
            }

            string digest = model.Digest;
            if (string.IsNullOrEmpty(digest) && card.StoreMode == CardStoreMode.Insert)
            {
                digest = await this.cardRepository.GetDigestAsync(card, CardDigestEventNames.CardModelClient);

                if (!string.IsNullOrEmpty(digest))
                {
                    model.Digest = digest;
                }
            }

            await this.printProvider.PrintDocumentAsync(digest, barcodeBytes);
        }

        private void SelectPrinterAndPrintAction(object parameter)
        {
            this.SelectPrinterAndPrintAsync(true);
        }

        #endregion

        #region Base Overrides

        public override Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            if (!this.licenseManager.License.Modules.Contains(LicenseModules.DocLoadID))
            {
                return Task.CompletedTask;
            }

            context.Workspace.LeftPanel.Tiles.Add(
                new Tile(
                    TileNames.PrintBarcode,
                    "$CardTypes_Controls_DocLoad_PrintBarcode",
                    context.Icons.Get("Thin181"),
                    context.Workspace.LeftPanel,
                    new DelegateCommand(this.PrintAction),
                    TileGroups.Cards,
                    order: 1000,
                    toolTip: TileHelper.GetToolTip("$CardTypes_Controls_DocLoad_PrintBarcode", TileKeys.PrintBarcode),
                    evaluating: EvaluatingPrintBarcode,
                    tiles: new TileCollection
                    {
                        new Tile(
                            TileNames.PrintBarcodeSelectPrinter,
                            TileHelper.SplitCaption("$CardTypes_Controls_DocLoad_SelectPrinterAndPrint"),
                            context.Icons.Get("Thin53"),
                            context.Workspace.LeftPanel,
                            new DelegateCommand(SelectPrinterAndPrintAction),
                            TileGroups.Cards,
                            order: 1,
                            toolTip: TileHelper.GetToolTip("$CardTypes_Controls_DocLoad_SelectPrinterAndPrint", TileKeys.PrintBarcodeSelectPrinter),
                            evaluating: EvaluatingSelectPrinter)
                    }));

            return Task.CompletedTask;
        }


        public override Task InitializingLocal(ITileLocalExtensionContext context)
        {
            ITile printBarcode = context.Workspace.LeftPanel.Tiles.TryGet(TileNames.PrintBarcode);
            if (printBarcode != null)
            {
                printBarcode.Context.AddInputBinding(printBarcode, TileKeys.PrintBarcode);

                ITile selectPrinter = printBarcode.Tiles.TryGet(TileNames.PrintBarcodeSelectPrinter);
                if (selectPrinter != null)
                {
                    selectPrinter.Context.AddInputBinding(selectPrinter, TileKeys.PrintBarcodeSelectPrinter);
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}