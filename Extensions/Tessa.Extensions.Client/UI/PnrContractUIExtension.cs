using System;
using System.Diagnostics;
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
    public sealed class PnrContractUIExtension : CardUIExtension
    {
        #region Constructors

        public PnrContractUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        #endregion

        #region Fields

        private readonly ICardRepository cardRepository;

        #endregion

        #region Private Methods
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
                cardModel.Card.Sections["PnrContracts"].Fields["SignatoryID"] = response.Info.Get<Guid?>("HeadLegalEntityID");
                cardModel.Card.Sections["PnrContracts"].Fields["SignatoryName"] = response.Info.Get<string>("HeadLegalEntityName");
            }
        }

        /// <summary>
        /// Валидация контролов.
        /// </summary>
        private void ChangeControlsActiveValidation(Guid kindID, ICardModel cardModel)
        {
            // Дата проекта/Дата заключения, Заголовок, КА: Договор с покупателями - валидации нет, остальные виды - есть
            if (cardModel.Controls.TryGet("Subject", out IControlViewModel subject))
            {
                subject.HasActiveValidation = kindID != PnrContractKinds.PnrContractWithBuyersID;
                subject.Rearrange();
            }
            if (cardModel.Controls.TryGet("Partner", out IControlViewModel partner))
            {
                partner.HasActiveValidation = kindID != PnrContractKinds.PnrContractWithBuyersID;
                partner.Rearrange();
            }
            if (cardModel.Controls.TryGet("ProjectDate", out IControlViewModel projectDate))
            {
                projectDate.HasActiveValidation = kindID != PnrContractKinds.PnrContractWithBuyersID;
                projectDate.Rearrange();
            }
        }


        /// <summary>
        /// Смена caption контролов, зависимых от Вида договора.
        /// </summary>
        private void ChangeControlsCaption(Guid kindID, ICardModel cardModel)
        {
            // Договор с покупателями
            // Предмет договора -> Заголовок
            if (cardModel.Controls.TryGet("Subject", out IControlViewModel subject))
            {
                subject.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Заголовок" : "Предмет договора";
            }
            // Дата заключения -> Дата проекта
            if (cardModel.Controls.TryGet("ProjectDate", out IControlViewModel projectDate))
            {
                projectDate.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Дата договора" : "Дата заключения";
            }
            // Внешний номер -> Номер договора
            if (cardModel.Controls.TryGet("ExternalNumber", out IControlViewModel externalNumber))
            {
                externalNumber.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Номер договора" : "Внешний номер";
            }
            // Дата начала -> Дата подписания
            if (cardModel.Controls.TryGet("StartDate", out IControlViewModel startDate))
            {
                startDate.Caption = kindID == PnrContractKinds.PnrContractWithBuyersID ? "Дата подписания" : "Дата начала";
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
        /// установка видимости контролов, зависимых от Вида договора.
        /// </summary>
        private void SetDKDependenceVisibility(Guid kindID, ICardModel cardModel)
        {
            // Договор с покупателями
            // Скрываем
            // ЦФО
            if (cardModel.Controls.TryGet("CFO", out IControlViewModel cfo))
            {
                cfo.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Наименование статьи затрат
            if (cardModel.Controls.TryGet("CostItem", out IControlViewModel costItem))
            {
                costItem.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Сумма договора (руб.)
            if (cardModel.Controls.TryGet("Amount", out IControlViewModel amount))
            {
                amount.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Сумма аванса (руб.)
            if (cardModel.Controls.TryGet("PrepaidExpenseAmount", out IControlViewModel prepaidExpenseAmount))
            {
                prepaidExpenseAmount.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Валюта расчета
            if (cardModel.Controls.TryGet("SettlementCurrency", out IControlViewModel settlementCurrency))
            {
                settlementCurrency.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // В бюджете
            if (cardModel.Controls.TryGet("IsInBudget", out IControlViewModel isInBudget))
            {
                isInBudget.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Проведен тендер
            if (cardModel.Controls.TryGet("IsTenderHeld", out IControlViewModel isTenderHeld))
            {
                isTenderHeld.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Тип договора
            if (cardModel.Controls.TryGet("Type", out IControlViewModel type))
            {
                type.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Отсрочка платежа (раб.дн.)
            if (cardModel.Controls.TryGet("DefermentPayment", out IControlViewModel defermentPayment))
            {
                defermentPayment.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Планируемая дата актирования
            if (cardModel.Controls.TryGet("PlannedActDate", out IControlViewModel plannedActDate))
            {
                plannedActDate.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Форма договора
            if (cardModel.Controls.TryGet("Form", out IControlViewModel form))
            {
                form.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Дата окончания
            if (cardModel.Controls.TryGet("EndDate", out IControlViewModel endDate))
            {
                endDate.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Подписант
            if (cardModel.Controls.TryGet("Signatory", out IControlViewModel signatory))
            {
                signatory.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Ставка НДС
            if (cardModel.Controls.TryGet("VATRate", out IControlViewModel VATRate))
            {
                VATRate.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Внутренний номер
            if (cardModel.Controls.TryGet("InternalNumber", out IControlViewModel internalNumber))
            {
                internalNumber.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Требуется согласование
            if (cardModel.Controls.TryGet("IsRequiresApproval", out IControlViewModel isRequiresApproval))
            {
                isRequiresApproval.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }
            // Вид договора 1С
            if (cardModel.Controls.TryGet("Kind1C", out IControlViewModel kind1C))
            {
                kind1C.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            }

            //// Вид договора
            //if (cardModel.Controls.TryGet("Kind", out IControlViewModel kind))
            //{
            //    kind.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Collapsed : Visibility.Visible;
            //    kind.Block.Rearrange();
            //}
            cardModel.Blocks["BasicInformationBlock"].Rearrange();
            cardModel.Blocks["BasicInformationBlock2"].Rearrange();
            cardModel.Blocks["BasicInformationBlock3"].Rearrange();
            cardModel.Blocks["BasicInformationBlock4"].Rearrange();

            // Контролы Требует согласования, Номер квартиры, Статус действия, Срочность
            IBlockViewModel additionalControlsBlock = cardModel.Blocks["AdditionalControlsBlock"];
            additionalControlsBlock.BlockVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;

            // В блоке Интеграция НСИ
            // Статус договора в CRM
            // if (cardModel.Controls.TryGet("CRMContractStatus", out IControlViewModel crmContractStatus))
            // {
            //     crmContractStatus.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;
            // }
            // Номер договора для МДМ
            if (cardModel.Controls.TryGet("MDMContractNumber", out IControlViewModel mdmContractNumber))
            {
                mdmContractNumber.ControlVisibility = kindID == PnrContractKinds.PnrContractWithBuyersID ? Visibility.Visible : Visibility.Collapsed;
            }
            cardModel.Blocks["IntegrationBlock"].Rearrange();

            IBlockViewModel contractDUPBlock = cardModel.Blocks["ContractDUPBlock"];
            contractDUPBlock.BlockVisibility = kindID == PnrContractKinds.PnrContractDUPID ? Visibility.Visible : Visibility.Collapsed;

            // в блоке ЦФО один контрол - Разработка договора, убран, значит и блок теряет смысл
            //IBlockViewModel contractCFOBlock = cardModel.Blocks["ContractCFOBlock"];
            //contractCFOBlock.BlockVisibility = kindID == PnrContractKinds.PnrContractCFOID ? Visibility.Visible : Visibility.Collapsed;
            //contractCFOBlock.Rearrange();
        }

        /// <summary>
        /// Установка гиперссылки в контрол.
        /// </summary>
        private async Task SetHyperlinkControls(Card card, ICardModel cardModel)
        {
            // Ссылка на карточку договора в CRM
            if (cardModel.Controls.TryGet("ContractLinkCrmControl", out IControlViewModel contractLinkCrmControl)
                && card.Sections["PnrContracts"].Fields["LinkCardCRM"] != null)
            {
                var linkControl = contractLinkCrmControl as HyperlinkViewModel;
                linkControl.Text = card.Sections["PnrContracts"].Fields["LinkCardCRM"].ToString();
                linkControl.LinkCommand = card.Sections["PnrContracts"].Fields["LinkCardCRM"].ToString();
                linkControl.ControlVisibility = Visibility.Visible;
            }
            // Гиперссылка на карточку
            if (cardModel.Controls.TryGet("ContractLinkCardControl", out IControlViewModel contractLinkCardControl)
                && card.Sections["PnrContracts"].Fields["HyperlinkCard"] != null)
            {
                var linkControl = contractLinkCardControl as HyperlinkViewModel;
                linkControl.Text = card.Sections["PnrContracts"].Fields["HyperlinkCard"].ToString();
                linkControl.LinkCommand = card.Sections["PnrContracts"].Fields["HyperlinkCard"].ToString();
                linkControl.ControlVisibility = Visibility.Visible;
            }
        }
        #endregion

        #region Base Overrides
        public override async Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Есть Вид договора - устанавливаем видимость контролов в зависимости от Вида
            if (card.Sections["PnrContracts"].Fields["KindID"] != null)
            {
                var kindID = (Guid)card.Sections["PnrContracts"].Fields["KindID"];
                SetDKDependenceVisibility(kindID, cardModel);
                ChangeControlsCaption(kindID, cardModel);
                ChangeControlsActiveValidation(kindID, cardModel);
            }
            if (card.Sections["PnrContracts"].Fields["KindDUPID"] != null)
            {
                SetDK_DUPDependenceVisibility((Guid)card.Sections["PnrContracts"].Fields["KindDUPID"], cardModel);
            }

            context.Card.Sections["PnrContracts"].FieldChanged += (s, e) =>
            {
                // Вид договора: Visibility контролов
                if (e.FieldName == "KindID" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility((Guid)e.FieldValue, cardModel);
                    ChangeControlsCaption((Guid)e.FieldValue, cardModel);
                    ChangeControlsActiveValidation((Guid)e.FieldValue, cardModel);
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
                    cardModel.Card.Sections["PnrContracts"].Fields["IsProjectInArchive"] = e.FieldValue;
                }
            };

            // Установка гиперссылки в контрол.
            await SetHyperlinkControls(card, cardModel);
        }
        #endregion
    }
}
