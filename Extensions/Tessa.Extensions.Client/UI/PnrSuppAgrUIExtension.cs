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
    public sealed class PnrSuppAgrUIExtension : CardUIExtension
    {
        public PnrSuppAgrUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }
        private readonly ICardRepository cardRepository;

        // Установка Подписант - руководитель выбранной организации
        private async void SetSignatory(Guid organizationID, ICardModel cardModel)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetOrganizationHead,
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
                cardModel.Card.Sections["PnrSupplementaryAgreements"].Fields["SignatoryID"] = response.Info.Get<Guid?>("HeadLegalEntityID");
                cardModel.Card.Sections["PnrSupplementaryAgreements"].Fields["SignatoryName"] = response.Info.Get<string>("HeadLegalEntityName");
            }
        }

        // установка видимости контролов
        private void SetAmountVisibility(object IsAmountChanged, ICardModel cardModel)
        {
            if (cardModel.Controls.TryGet("Amount", out IControlViewModel amount) && cardModel.Controls.TryGet("AmountSA", out IControlViewModel amountSA))
            {
                amount.ControlVisibility = (Boolean)IsAmountChanged ? Visibility.Visible : Visibility.Collapsed;
                amountSA.ControlVisibility = (Boolean)IsAmountChanged ? Visibility.Visible : Visibility.Collapsed;
                amount.Block.Rearrange();
            }
        }

        /// <summary>
        /// установка видимости контролов, зависимых от Вида договора (ДУП).
        /// </summary>
        private void SetDK_DUPDependenceVisibility(Guid kindDUPID, ICardModel cardModel)
        {
            // Доп.согласующие при Вид договора (ДУП) == "Нестроительный"
            if (cardModel.Controls.TryGet("ApprovingPersons", out IControlViewModel approvingPersons))
            {
                approvingPersons.ControlVisibility = kindDUPID == PnrContractKinds.PnrContractDUPNotBuildingID ? Visibility.Visible : Visibility.Collapsed;
                approvingPersons.Block.Rearrange();
            }
        }
        
        /// <summary>
        /// Смена caption контролов, зависимых от Вида дополнительного соглашения.
        /// </summary>
        private void ChangeControlsCaption(Guid kindID, ICardModel cardModel)
        {
            // Договор с покупателями

            // Дата заключения -> Дата договора
            if (cardModel.Controls.TryGet("ProjectDate", out IControlViewModel projectDate))
            {
                projectDate.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Дата договора" : "Дата заключения";
            }
            // Дата начала -> Дата подписания
            if (cardModel.Controls.TryGet("StartDate", out IControlViewModel startDate))
            {
                startDate.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Дата подписания" : "Дата начала";
            }
            // Предмет договора -> Заголовок
            if (cardModel.Controls.TryGet("Subject", out IControlViewModel subject))
            {
                subject.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Заголовок" : "Предмет договора";
            }
            // Внешний номер -> Номер договора
            if (cardModel.Controls.TryGet("ExternalNumber", out IControlViewModel externalNumber))
            {
                externalNumber.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Номер договора" : "Внешний номер";
            }
        }

        private void SetDKDependenceVisibility(object kindID, ICardModel cardModel)
        {
            IBlockViewModel suppAgrDUPBlock = cardModel.Blocks["SuppAgrDUPBlock"];
            suppAgrDUPBlock.BlockVisibility = (Guid)kindID == PnrContractKinds.PnrContractDUPID ? Visibility.Visible : Visibility.Collapsed;
            suppAgrDUPBlock.Rearrange();

            // Включить видимость блоков, если Вид договора - С покупателем
            IBlockViewModel suppAgrWithBuyersBlock = cardModel.Blocks["SuppAgrWithBuyersBlock"];
            suppAgrWithBuyersBlock.BlockVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;
            suppAgrDUPBlock.Rearrange();

            IBlockViewModel integrationBlock = cardModel.Blocks["IntegrationBlock"];
            integrationBlock.BlockVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;
            integrationBlock.Rearrange();

            // Убрать видимость полей, если Вид договора - С покупателем
            // В бюджет
            if (cardModel.Controls.TryGet("IsInBudget", out IControlViewModel isInBudget))
            {
                isInBudget.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Проведен тендер
            if (cardModel.Controls.TryGet("IsTenderHeld", out IControlViewModel isTenderHeld))
            {
                isTenderHeld.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // ЦФО
            if (cardModel.Controls.TryGet("CFO", out IControlViewModel CFO))
            {
                CFO.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Наименование статьи затрат
            if (cardModel.Controls.TryGet("CostItem", out IControlViewModel costItem))
            {
                costItem.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Тип договора
            if (cardModel.Controls.TryGet("Type", out IControlViewModel type))
            {
                type.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Подписант
            if (cardModel.Controls.TryGet("Signatory", out IControlViewModel signatory))
            {
                signatory.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Внутренний номер
            if (cardModel.Controls.TryGet("InternalNumber", out IControlViewModel internalNumber))
            {
                internalNumber.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Причина заключения ДС
            if (cardModel.Controls.TryGet("Reason", out IControlViewModel reason))
            {
                reason.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Изменение суммы договора
            if (cardModel.Controls.TryGet("IsAmountChanged", out IControlViewModel isAmountChanged))
            {
                isAmountChanged.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            //// Сумма ДС (руб.)
            //if (cardModel.Controls.TryGet("AmountSA", out IControlViewModel amountSA))
            //{
            //    amountSA.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            //}
            // Сумма аванса (руб.)
            if (cardModel.Controls.TryGet("PrepaidExpenseAmount", out IControlViewModel prepaidExpenseAmount))
            {
                prepaidExpenseAmount.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Валюта расчета
            if (cardModel.Controls.TryGet("SettlementCurrency", out IControlViewModel settlementCurrency))
            {
                settlementCurrency.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Ставка НДС
            if (cardModel.Controls.TryGet("VATRate", out IControlViewModel VATRate))
            {
                VATRate.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Форма ДС
            if (cardModel.Controls.TryGet("Form", out IControlViewModel form))
            {
                form.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Дата окончания
            if (cardModel.Controls.TryGet("EndDate", out IControlViewModel endDate))
            {
                endDate.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Основной договор
            if (cardModel.Controls.TryGet("MainContract", out IControlViewModel mainContract))
            {
                mainContract.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Основной договор заключен до 2019
            if (cardModel.Controls.TryGet("IsUntil2019", out IControlViewModel isUntil2019))
            {
                isUntil2019.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Согласование бухгалтерией
            if (cardModel.Controls.TryGet("CRMApprove", out IControlViewModel CRMApprove))
            {
                CRMApprove.ControlVisibility = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;
            }

            // Убрать признак обязательного заполнения полей, если Вид договора - С покупателем
            // Сумма ДС
            //if (cardModel.Controls.TryGet("AmountSA", out amountSA))
            //{
            //    amountSA.IsRequired = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? false : true;
            //    amountSA.HasActiveValidation = amountSA.IsRequired;
            //    amountSA.Rearrange();
            //}
            // Форма ДС
            if (cardModel.Controls.TryGet("Form", out form))
            {
                form.IsRequired = (Guid)kindID == PnrContractKinds.PnrContractWithBuyersID ? false : true;
                form.HasActiveValidation = form.IsRequired;
                form.Rearrange();
            }

            //IBlockViewModel suppAgrCFOBlock = cardModel.Blocks["SuppAgrCFOBlock"];
            //suppAgrCFOBlock.BlockVisibility = (Guid)kindID == PnrContractKinds.PnrContractCFOID ? Visibility.Visible : Visibility.Collapsed;
            //suppAgrCFOBlock.Rearrange();

            foreach (var block in cardModel.BlockBag)
            {
                block.Rearrange();
            }
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Есть Вид договора - устанавливаем видимость контролов в зависимости от Вида
            if (card.Sections["PnrSupplementaryAgreements"].Fields["KindID"] != null)
            {
                var kindID = card.Sections["PnrSupplementaryAgreements"].Fields["KindID"];
                SetDKDependenceVisibility(kindID, cardModel);
                ChangeControlsCaption((Guid)kindID, cardModel);
            }
            if (card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPID"] != null)
            {
                SetDK_DUPDependenceVisibility((Guid)card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPID"], cardModel);
            }

            // Открытие карточки
            if (card.StoreMode == CardStoreMode.Update)
            {
                // Есть Изменение суммы договора - устанавливаем видимость Сумма договора с учетом ДС
                if (card.Sections["PnrSupplementaryAgreements"].Fields["IsAmountChanged"] != null)
                {
                    object isAmountChanged = card.Sections["PnrSupplementaryAgreements"].Fields["IsAmountChanged"];
                    if ((Boolean)isAmountChanged)
                        SetAmountVisibility(isAmountChanged, cardModel);
                }
            }

            IBlockViewModel cBlock = null;
            if(cardModel.Controls.TryGet("Block5", out IControlViewModel control))
            { 
                cBlock = control as IBlockViewModel;
            }

            if (cardModel.Controls.TryGet("MainContract", out IControlViewModel mainContract) && cardModel.Controls.TryGet("IsUntil2019", out IControlViewModel isUntil2019))
            {
                if (((CheckBoxViewModel) isUntil2019).IsChecked)
                {
                    mainContract.ControlVisibility = Visibility.Collapsed;
                    card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"] = null;
                    card.Sections["PnrSupplementaryAgreements"].Fields["MainContractSubject"] = null;
                    cBlock?.Rearrange();
                }
            }

            context.Card.Sections["PnrSupplementaryAgreements"].FieldChanged += (s, e) =>
            {
                // Изменение суммы договора: Visibility Сумма договора с учетом ДС
                if (e.FieldName == "IsAmountChanged" && e.FieldValue != null)
                {
                    SetAmountVisibility(e.FieldValue, cardModel);
                }

                // Если устанавливаем галку договор до 2019 года то очищаем секцию с основным договором
                if (e.FieldName == "IsUntil2019" && e.FieldValue != null && (bool)e.FieldValue)
                {
                    card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"] = null;
                    card.Sections["PnrSupplementaryAgreements"].Fields["MainContractSubject"] = null;
                    mainContract.ControlVisibility = Visibility.Collapsed;
                    cBlock?.Rearrange();
                }
                else
                {
                    mainContract.ControlVisibility = Visibility.Visible;
                    cBlock?.Rearrange();
                }

                // И наоборот если выбираем основной договор то убирается галочка договор 2019
                if (e.FieldName == "MainContractID" && e.FieldValue != null)
                {
                    card.Sections["PnrSupplementaryAgreements"].Fields["IsUntil2019"] = false;
                }


                // Вид договора: Visibility блоков
                if (e.FieldName == "KindID" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility(e.FieldValue, cardModel);
                    ChangeControlsCaption((Guid)e.FieldValue, cardModel);
                }

                // Организация ГК Пионер: заполнение Подписант
                if (e.FieldName == "OrganizationID" && e.FieldValue != null)
                {
                    SetSignatory((Guid)e.FieldValue, cardModel);
                }

                // Вид договора (ДУП): Visibility контролов
                if (e.FieldName == "KindDUPID" && e.FieldValue != null)
                {
                    SetDK_DUPDependenceVisibility((Guid)e.FieldValue, cardModel);
                }

                // Проект: заполняем скрытое поле на карточке (используется в Критериях процесса)
                if (e.FieldName == "ProjectInArchive")
                {
                    cardModel.Card.Sections["PnrSupplementaryAgreements"].Fields["IsProjectInArchive"] = e.FieldValue;
                }
            };

            return base.Initialized(context);
        }
    }
}
