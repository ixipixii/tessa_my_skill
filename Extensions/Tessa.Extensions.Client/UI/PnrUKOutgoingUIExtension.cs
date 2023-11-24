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

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrUKOutgoingUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrUKOutgoingUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

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
                //cardModel.Card.Sections["PnrOutgoingUK"].Fields["LegalEntityIndexID"] = response.Info.Get<Guid?>("LegalEntityID");
                cardModel.Card.Sections["PnrOutgoingUK"].Fields["LegalEntityIndexIdx"] = response.Info.Get<string>("LegalEntityIdx");
            }
        }

        private async void SetUserDepartmentInfo(Guid? authorID, ICardModel cardModel)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
                Info =
                {
                    { "authorID", authorID }
                }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
            TessaDialog.ShowNotEmpty(result);
            if (result.IsSuccessful && cardModel.Card.Sections["PnrOutgoingUK"].Fields["DepartmentID"] == null)
            {
                cardModel.Card.Sections["PnrOutgoingUK"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                cardModel.Card.Sections["PnrOutgoingUK"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                cardModel.Card.Sections["PnrOutgoingUK"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
            }
        }

        // установка видимости контролов, зависимых от Вида исходящего документа УК ПС
        private void SetDKDependenceVisibility(string documentKindIdx, ICardModel cardModel)
        {
            IBlockViewModel incomingDocsBlock = cardModel.Blocks["OutgoingDocsBlock"];
            incomingDocsBlock.BlockVisibility = documentKindIdx == PnrOutgoingUKTypes.OutgoingUKLetterIdx ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel complaintsBlock = cardModel.Blocks["ComplaintsBlock"];
            complaintsBlock.BlockVisibility = documentKindIdx == PnrOutgoingUKTypes.OutgoingUKComplaintsIdx ? Visibility.Visible : Visibility.Collapsed;
            complaintsBlock.Rearrange();
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel projectDateControl))
                {
                    ((DateTimeViewModel)projectDateControl).SelectedDate = DateTime.Now;
                }

                // начальная инициализация поля Подразделение по автору
                SetUserDepartmentInfo((Guid?)card.Sections["DocumentCommonInfo"].Fields["AuthorID"], cardModel);

                // поле Вид исходящего документа УК автозополнено - устанавливаем видимость контролов в зависимости от Вида
                SetDKDependenceVisibility((string)card.Sections["PnrOutgoingUK"].Fields["DocumentKindIdx"], cardModel);
            }
            else if (card.StoreMode == CardStoreMode.Update && card.Sections["PnrOutgoingUK"].Fields["DocumentKindID"] != null)
            // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
            {
                SetDKDependenceVisibility((string)card.Sections["PnrOutgoingUK"].Fields["DocumentKindIdx"], cardModel);
            }

            context.Card.Sections["PnrOutgoingUK"].FieldChanged += (s, e) =>
            {
                // Вид исходящего документа: Visibility контролов
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
