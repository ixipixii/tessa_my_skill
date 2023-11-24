using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrRegulationUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrRegulationUIExtension(ICardRepository cardRepository)
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
                cardModel.Card.Sections["PnrRegulations"].Fields["CFOID"] = response.Info.Get<Guid?>("CFOID");
                cardModel.Card.Sections["PnrRegulations"].Fields["CFOName"] = response.Info.Get<string>("CFOName");
            }
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            if(card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel regDateControl))
                {
                    ((DateTimeViewModel)regDateControl).SelectedDate = DateTime.Now;
                }
                // ЦФО(по Инициатору) при создании карточки
                SetUserDepartmentInfo((Guid)card.Sections["DocumentCommonInfo"].Fields["AuthorID"], cardModel);
            }

            return base.Initialized(context);
        }
    }
}
