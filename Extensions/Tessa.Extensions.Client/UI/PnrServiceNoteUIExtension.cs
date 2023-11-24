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
    public sealed class PnrServiceNoteUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrServiceNoteUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        //private async void SetLegalEntityIndex(Guid organizationID, ICardModel cardModel)
        //{
        //    CardRequest request = new CardRequest
        //    {
        //        RequestType = Shared.PnrRequestTypes.GetIndexLegalEntityRequestTypeID,
        //        Info =
        //        {
        //            { "organizationID", organizationID }
        //        }
        //    };

        //    CardResponse response = await cardRepository.RequestAsync(request);

        //    Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
        //    TessaDialog.ShowNotEmpty(result);
        //    if (result.IsSuccessful)
        //    {
        //        ///cardModel.Card.Sections["PnrServiceNote"].Fields["LegalEntityIndexID"] = response.Info.Get<Guid?>("LegalEntityID"); ;
        //        cardModel.Card.Sections["PnrServiceNote"].Fields["LegalEntityIndexIdx"] = response.Info.Get<string>("LegalEntityIdx");
        //    }
        //}

        // установка видимости блоков, зависимых от Типа служебной записки
        private void SetNTDependenceVisibility(Guid? serviceNoteTypeID, ICardModel cardModel)
        {
            IBlockViewModel personnelBlock = cardModel.Blocks["PersonnelBlock"];
            personnelBlock.BlockVisibility = serviceNoteTypeID == PnrServiceNoteTypes.Personnel ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel conclusionContractsBlock = cardModel.Blocks["ConclusionContractsBlock"];
            conclusionContractsBlock.BlockVisibility = serviceNoteTypeID == PnrServiceNoteTypes.ConclusionContracts ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel conclusionContractsDUPBlock = cardModel.Blocks["ConclusionContractsDUPBlock"];
            conclusionContractsDUPBlock.BlockVisibility = serviceNoteTypeID == PnrServiceNoteTypes.ConclusionContractsDUP ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel financialActivitiesBlock = cardModel.Blocks["FinancialActivitiesBlock"];
            financialActivitiesBlock.BlockVisibility = serviceNoteTypeID == PnrServiceNoteTypes.FinancialActivities ? Visibility.Visible : Visibility.Collapsed;

            IBlockViewModel workQuestionsBlock = cardModel.Blocks["WorkQuestionsBlock"];
            workQuestionsBlock.BlockVisibility = serviceNoteTypeID == PnrServiceNoteTypes.WorkQuestions ? Visibility.Visible : Visibility.Collapsed;

            workQuestionsBlock.Rearrange();
        }

        private void ChangeControlsActiveValidation(Guid? serviceNoteTypeID, ICardModel cardModel)
        {
            if (cardModel.Controls.TryGet("ConclusionContractsTheme", out IControlViewModel ContractsThemeControl) &&
                cardModel.Controls.TryGet("FinancialActivitiesTheme", out IControlViewModel ActivitiesThemeControl))
            {
                ContractsThemeControl.IsRequired = serviceNoteTypeID == PnrServiceNoteTypes.ConclusionContracts;
                ActivitiesThemeControl.IsRequired = serviceNoteTypeID == PnrServiceNoteTypes.FinancialActivities;
                ContractsThemeControl.NotifyUpdateValidation();
                ActivitiesThemeControl.NotifyUpdateValidation();
            }
            
        }

        private async void SetUserDepartmentInfo(Guid? authorID, ICardModel cardModel, Boolean isInitiator)
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
                if (isInitiator)
                {
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
                }
                else
                {
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DestinationDepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DestinationDepartmentName"] = response.Info.Get<string>("Name");
                    cardModel.Card.Sections["PnrServiceNote"].Fields["DestinationDepartmentIdx"] = response.Info.Get<string>("Index");
                }
            }
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel regDateControl))
                {
                    ((DateTimeViewModel)regDateControl).SelectedDate = DateTime.Now;
                }
                // начальная инициализация поля Подразделение по автору
                SetUserDepartmentInfo((Guid?)card.Sections["DocumentCommonInfo"].Fields["AuthorID"], cardModel, true);
            } 
            else
            {
                // Открытие карточки. Есть Тип служебной записки - устанавливаем видимость блоков в зависимости от Типа
                SetNTDependenceVisibility((Guid?)card.Sections["PnrServiceNote"].Fields["ServiceNoteTypeID"], cardModel);
            }

            context.Card.Sections["PnrServiceNote"].FieldChanged += (s, e) =>
            {
                // Тип служебной записки: Visibility блоков
                if (e.FieldName == "ServiceNoteTypeID" && e.FieldValue != null)
                {
                    SetNTDependenceVisibility((Guid)e.FieldValue, cardModel);
                    ChangeControlsActiveValidation((Guid)e.FieldValue, cardModel);

                }

                // Организация ГК Пионер: заполнение Индекс ЮЛ (но он далее нигде не используется)
                //if (e.FieldName == "OrganizationID" && e.FieldValue != null)
                //{
                //    SetLegalEntityIndex((Guid)e.FieldValue, cardModel);
                //}

                // Адресат: подчитываем его подразделение
                if (e.FieldName == "DestinationID" && e.FieldValue != null)
                {
                    SetUserDepartmentInfo((Guid?)e.FieldValue, cardModel, false);
                }
            };

            return base.Initialized(context);
        }
    }
}
