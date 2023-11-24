using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Cards.Controls.AutoComplete;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrUKIncomingUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrUKIncomingUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        // установка видимости контролов, зависимых от Вида входящего документа УК ПС
        private void SetDKDependenceVisibility(string documentKindIdx, ICardModel cardModel)
        {
            IBlockViewModel incomingDocsBlock = cardModel.Blocks["IncomingDocsBlock"];
            incomingDocsBlock.BlockVisibility = documentKindIdx == "0-01" ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel complaintsBlock = cardModel.Blocks["ComplaintsBlock"];
            complaintsBlock.BlockVisibility = documentKindIdx == "0-02" ? Visibility.Visible : Visibility.Collapsed;
            complaintsBlock.Rearrange();
        }

        // установка значения скрытого поля Индекс ЮЛ
        private async void SetLegalEntityIndex(Guid organizationID, ICardModel cardModel)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetIndexLegalEntityRequestTypeID,
                Info =
                {
                    { "organizationID", organizationID }
                }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
            TessaDialog.ShowNotEmpty(result);
            if (result.IsSuccessful)
            {
                //cardModel.Card.Sections["PnrIncomingUK"].Fields["LegalEntityIndexID"] = response.Info.Get<Guid?>("LegalEntityID");
                cardModel.Card.Sections["PnrIncomingUK"].Fields["LegalEntityIndexIdx"] = response.Info.Get<string>("LegalEntityIdx");
            }
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel projectDateControl))
                {
                    ((DateTimeViewModel)projectDateControl).SelectedDate = DateTime.Now;
                }

                // поле Вид входящего документа УК автозополнено - устанавливаем видимость контролов в зависимости от Вида
                SetDKDependenceVisibility((string)card.Sections["PnrIncomingUK"].Fields["DocumentKindIdx"], cardModel);
            }
            else if (card.StoreMode == CardStoreMode.Update && card.Sections["PnrIncomingUK"].Fields["DocumentKindID"] != null)
            // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
            {
                SetDKDependenceVisibility((string)card.Sections["PnrIncomingUK"].Fields["DocumentKindIdx"], cardModel);
            }

            context.Card.Sections["PnrIncomingUK"].FieldChanged += (s, e) =>
            {
                // Вид входящего документа: Visibility контролов
                if (e.FieldName == "DocumentKindIdx" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility((string)e.FieldValue, cardModel);
                }

                // Организация ГК Пионер: заполнение Индекс ЮЛ
                if (e.FieldName == "OrganizationID" && e.FieldValue != null)
                {
                    SetLegalEntityIndex((Guid)e.FieldValue, cardModel);
                }
            };

            return base.Initialized(context);
        }
    }
}
