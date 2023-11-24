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
    public sealed class PnrOrderUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrOrderUIExtension(ICardRepository cardRepository)
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
                cardModel.Card.Sections["PnrOrder"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                cardModel.Card.Sections["PnrOrder"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                cardModel.Card.Sections["PnrOrder"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
            }
        }

        // установка видимости контролов, зависимых от Вида документа
        private void SetDKDependenceVisibility(Guid? documentKindID, ICardModel cardModel)
        {
            //IBlockViewModel orderAdministrativeActivityBlock = cardModel.Blocks["OrderAdministrativeActivityBlock"];
            //orderAdministrativeActivityBlock.BlockVisibility = documentKindID == PnrDocumentKinds.OrderAdministrativeActivity ? Visibility.Visible : Visibility.Collapsed;

            //IBlockViewModel orderMainActivityBlock = cardModel.Blocks["OrderMainActivityBlock"];
            //orderMainActivityBlock.BlockVisibility = documentKindID == PnrDocumentKinds.OrderMainActivity ? Visibility.Visible : Visibility.Collapsed;

            //IBlockViewModel orderMobileCommunicationsBlock = cardModel.Blocks["OrderMobileCommunicationsBlock"];
            //orderMobileCommunicationsBlock.BlockVisibility = documentKindID == PnrDocumentKinds.OrderMobileCommunications ? Visibility.Visible : Visibility.Collapsed;

            //IBlockViewModel orderImplementationBlock = cardModel.Blocks["OrderImplementationBlock"];
            //orderImplementationBlock.BlockVisibility = documentKindID == PnrDocumentKinds.OrderImplementation ? Visibility.Visible : Visibility.Collapsed;

            //IBlockViewModel disposalBlock = cardModel.Blocks["DisposalBlock"];
            //disposalBlock.BlockVisibility = documentKindID == PnrDocumentKinds.Disposal ? Visibility.Visible : Visibility.Collapsed;

            //disposalBlock.Rearrange();

            IBlockViewModel disposalBlock = cardModel.Blocks["DisposalBlock"];
            if (documentKindID == PnrDocumentKinds.OrderAdministrativeActivity ||
                documentKindID == PnrDocumentKinds.OrderMainActivity ||
                documentKindID == PnrDocumentKinds.OrderMobileCommunications ||
                documentKindID == PnrDocumentKinds.OrderImplementation)
            {
                disposalBlock.BlockVisibility = Visibility.Collapsed;
            }
            else if (documentKindID == PnrDocumentKinds.Disposal)
            {
                disposalBlock.BlockVisibility = Visibility.Visible;
            }
            disposalBlock.Rearrange();

        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel regDateControl))
                {
                    ((DateTimeViewModel)regDateControl).SelectedDate = DateTime.Now;
                }

                // начальная инициализация поля Подразделение по автору
                SetUserDepartmentInfo((Guid?)card.Sections["DocumentCommonInfo"].Fields["AuthorID"], cardModel);
            }
            // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
            else
            {
                Guid? documentKindID = (Guid?)card.Sections["PnrOrder"].Fields["DocumentKindID"];
                if (documentKindID != null)
                    SetDKDependenceVisibility(documentKindID, cardModel);
            }

            context.Card.Sections["PnrOrder"].FieldChanged += (s, e) =>
            {
                // Вид документа: Visibility контролов и переформирование номера
                if (e.FieldName == "DocumentKindID" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility((Guid)e.FieldValue, cardModel);
                }
            };

            return base.Initialized(context);
        }
    }
}
