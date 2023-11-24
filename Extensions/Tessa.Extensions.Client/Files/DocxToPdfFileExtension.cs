using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.FileConverters;
using Tessa.Files;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Files;
using Tessa.UI.Menu;

using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.Files
{
    // нужно еще расширение файла docx отлавливать и открывать только на нем
    public sealed class DocxToPdfFileExtension : FileExtension
    {
        private readonly ICardRepository cardRepository;
        public DocxToPdfFileExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        public override async Task OpeningMenu(IFileExtensionContext context)
        {
            ICardEditorModel editor = UIContext.Current.CardEditor;
            var uiContext = UIContext.Current.CardEditor.Context;
            ICardModel model;
            if (editor == null
                || (model = editor.CardModel) == null
                //|| model.CardType.Name != ""
                )
            {
                return;
            }

            // Добавляем пункт меню для получения файлов
            context.Actions.AddRange(
                new MenuAction(
                    "DocxToPdf",
                    "Конвертировать в pdf",
                    context.Icons.Get("Thin427"),
                    new DelegateCommand(async o => ConvertDocxFileToPdfMultiple(cardRepository, editor, context.Control, context.Files))
                    //,
                    //isSelectable: true,
                    //isSelected: isContainsExternalFiles
                    )
            );
        }

        // convert
        private static async Task ConvertDocxFileToPdfMultiple(ICardRepository cardRepository, ICardEditorModel editor, IFileControl control, IFileCollection files)
        {
            using (var splash = TessaSplash.Create("Конвертация файлов в pdf..."))
            {
                Guid cardID = editor.CardModel.Card.ID;
                string notConvertedDocxFiles = "";
                string notSavedDocxFiles = "";

                for (int i = 0; i < files.Count; i++)
                {
                    int versionsCount = files[i].Versions.Count;
                    if (files[i].Versions[versionsCount - 1].State == Tessa.Files.FileVersionState.Created)
                    {
                        if (!string.IsNullOrEmpty(notSavedDocxFiles))
                            notSavedDocxFiles += ", ";
                        else notSavedDocxFiles += (files[i].Name + " ");
                    }
                }
                if (!string.IsNullOrEmpty(notSavedDocxFiles))
                {
                    TessaDialog.ShowMessage($"Файлы: {notSavedDocxFiles} добавлены в карточку, но не сохранены. Сохраните карточку перед конвертацией файлов.", "Конвертация в pdf");
                    return;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    int versionsCount = files[i].Versions.Count;
                    Guid versionRowID = files[i].Versions[versionsCount - 1].ID;

                    CardRequest request = new CardRequest
                    {
                        RequestType = Shared.PnrRequestTypes.GetPdfFromDocxTypeID,
                        Info =
                    {
                        { "versionRowID", versionRowID },
                        { "cardID", cardID },
                        { "fileName", files[i].Name }
                    }
                    };

                    CardResponse response = await cardRepository.RequestAsync(request);

                    Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
                    if (!result.IsSuccessful || !response.Info.Get<bool>("ConvertFileResult"))
                    {
                        if (!string.IsNullOrEmpty(notConvertedDocxFiles))
                            notConvertedDocxFiles += ", ";
                        else notConvertedDocxFiles += (files[i].Name + " ");
                    }
                }

                if (!string.IsNullOrEmpty(notConvertedDocxFiles))
                {
                    TessaDialog.ShowError($"Конвертация файлов: {notConvertedDocxFiles}в pdf завершилась с ошибкой.", "Конвертация в pdf");
                }

                await editor.RefreshCardAsync(editor.Context);
            }
        }
    }
}