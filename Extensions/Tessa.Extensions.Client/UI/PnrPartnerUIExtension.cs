using System;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Files;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrPartnerUIExtension : CardUIExtension
    {
        private readonly IUIHost uiHost;

        public PnrPartnerUIExtension(IUIHost uiHost)
        {
            this.uiHost = uiHost;
        }

        // Установка видимости контролов, зависимых от Типа контрагента
        private void SetBlockVisibility(ICardModel cardModel, int? partnerType)
        {
            var blockLegalEntity = cardModel.Blocks["BlockLegalEntity"];
            var isLegalEntityOrSoleTrader = partnerType == 1 || partnerType == 3;
            blockLegalEntity.BlockVisibility = isLegalEntityOrSoleTrader ? Visibility.Visible : Visibility.Collapsed;
            blockLegalEntity.Rearrange();

            var blockIndividual = cardModel.Blocks["BlockIndividual"];
            var isIndividual = partnerType == 2;
            blockIndividual.BlockVisibility = isIndividual ? Visibility.Visible : Visibility.Collapsed;
            blockIndividual.Rearrange();

            // блоки-подсказки перечня категорий файлов по типам КА
            // для Юридического лица
            var fileHelperLegalEntityIndividual = cardModel.Blocks["FileHelperLegalEntity"];
            fileHelperLegalEntityIndividual.BlockVisibility = partnerType == 1 ? Visibility.Visible : Visibility.Collapsed;
            fileHelperLegalEntityIndividual.Rearrange();
            // для Индивидуального предпринимателя
            var fileHelperSoleTrader = cardModel.Blocks["FileHelperSoleTrader"];
            fileHelperSoleTrader.BlockVisibility = partnerType == 3 ? Visibility.Visible : Visibility.Collapsed;
            fileHelperSoleTrader.Rearrange();
            // для Физического лица
            var fileHelperIndividual = cardModel.Blocks["FileHelperIndividual"];
            fileHelperIndividual.BlockVisibility = partnerType == 2 ? Visibility.Visible : Visibility.Collapsed;
            fileHelperIndividual.Rearrange();
        }

        /// <summary>
        /// Обработчик вирт. таблицы "Заявки". При двойном клике по строке таблицы открывает связанную карточку.
        /// </summary>
        private void RefRequestsVirtualHandler(ICardModel model)
        {
            if (model.Controls.TryGet("RefRequestsVirtualControl", out var refRequestsVirtualControl)
                && refRequestsVirtualControl is GridViewModel refRequestsVirtualControlGrid)
            {
                refRequestsVirtualControlGrid.RowInvoked += async (s, e) =>
                {
                    //событие открытия записи
                    if (e.Action == GridRowAction.Opening)
                    {
                        // отменяем открытие формы редактирования
                        e.Cancel = true;

                        var requestDocID = e.Row.Fields.TryGet<Guid?>("DocID");

                        if (requestDocID != null)
                        {
                            using ISplash splash = TessaSplash.Create(TessaSplashMessage.OpeningCard);

                            // откроем карточку
                            await this.uiHost.OpenCardAsync(
                                cardID: requestDocID.Value,
                                cardTypeID: PnrCardTypes.PnrPartnerRequestTypeID,
                                options: new OpenCardOptions
                                {
                                    UIContext = UIContext.Current,
                                    Splash = splash
                                });
                        }
                    }
                };
            }
        }

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            var cardSection = context.Card.Sections["Partners"];
            var cardModel = context.Model;
            SetBlockVisibility(cardModel, (int?)cardSection.Fields["TypeID"]);

            cardSection.FieldChanged += (s, e) =>
            {
                if (e.FieldName == "TypeID")
                {
                    SetBlockVisibility(cardModel, (int?)cardSection.Fields["TypeID"]);
                }
            };

            // Логика вирт .таблицы "Заявки"
            RefRequestsVirtualHandler(cardModel);

            if (context.Model.Controls.TryGet("Files",
                out IControlViewModel FileListModel))
            {
                FileListViewModel FileList = (FileListViewModel) FileListModel;
                foreach (var file in FileList.FileControl.Files)
                {
                    ChangeHelperControlVisibility(cardModel, new FileControlEventArgs(FileList.FileControl.Container, FileList.FileControl, file), Visibility.Collapsed);
                }

                FileList.FileControl.ContainerFileAdded += (sender, e) =>
                {
                    ChangeHelperControlVisibility(cardModel, e, Visibility.Collapsed);
                        
                };
                FileList.FileControl.ContainerFileRemoved += (sender, e) =>
                {
                    ChangeHelperControlVisibility(cardModel, e, Visibility.Visible);
                };
            }
        }

        private void ChangeHelperControlVisibility(ICardModel cardModel, FileControlEventArgs e, Visibility visibility)
        {
            IBlockViewModel currentHelpBlock = GetCurrentHelperBlock(cardModel);
            if(currentHelpBlock == null) return;

            foreach (var control in currentHelpBlock.Controls)
            {
                if (control.CardTypeControl.Type.Name == "Label")
                {
                    if (e.File.Category != null && ((LabelViewModel)control).Text.Contains(e.File.Category.Caption))
                    {
                        control.ControlVisibility = visibility;
                    }
                }
            }
        }

        private IBlockViewModel GetCurrentHelperBlock(ICardModel cardModel)
        {
            if (cardModel.Blocks["FileHelperLegalEntity"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperLegalEntity"];
            }
            else if (cardModel.Blocks["FileHelperSoleTrader"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperSoleTrader"];
            }
            else if (cardModel.Blocks["FileHelperIndividual"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperIndividual"];
            }
            else
            {
                return null;
            }
        }
    }
}