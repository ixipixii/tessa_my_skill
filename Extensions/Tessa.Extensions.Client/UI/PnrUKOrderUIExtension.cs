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
    public sealed class PnrUKOrderUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrUKOrderUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
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
            if (result.IsSuccessful)
            {
                cardModel.Card.Sections["PnrOrderUK"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                cardModel.Card.Sections["PnrOrderUK"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                cardModel.Card.Sections["PnrOrderUK"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
            }
        }

        private async void SetLegalEntityIndex(Guid organizationID, ICardModel cardModel)
        {
            // request к организации получить индекс ЮЛ
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
                //cardModel.Card.Sections["PnrOrderUK"].Fields["LegalEntityIndexID"] = response.Info.Get<Guid?>("LegalEntityID"); ;
                cardModel.Card.Sections["PnrOrderUK"].Fields["LegalEntityIndexIdx"] = response.Info.Get<string>("LegalEntityIdx");
            }
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
            }

            context.Card.Sections["PnrOrderUK"].FieldChanged += (s, e) =>
            {
                // Организация ГК Пионер: заполнение Индекс ЮЛ + изменение Номера
                if (e.FieldName == "OrganizationID" && e.FieldValue != null)
                {
                    SetLegalEntityIndex((Guid)e.FieldValue, cardModel);
                }
            };

            return base.Initialized(context);
        }
    }
}