using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Server.Integration.Models;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Senders
{
    public class PnrContractMdmSender : PnrMdmBaseSender
    {
        public PnrContractMdmSender(IDbScope dbScope, ICardRepository cardRepository, ICardCache cardCache, IValidationResultBuilder validationResult, ILogger logger, Card card, ISession session) : base(dbScope, cardRepository, cardCache, validationResult, logger, card, session)
        {
        }

        /// <summary>
        /// Возвращает признак изменения хотя бы одного из отслеживаемых полей договора:
        /// После успешного создания договора, на стороне СЭД возможно изменение следующий атрибутов договора, влекущих передачу информации в НСИ:
        /// - ЦФО
        /// - Проект
        /// - Внешний или внутренний номер
        /// - Статья затрат
        /// </summary>
        private bool IsContractTrackedFieldsChanged(Card card)
        {
            // если в карточке нет изменений, выходим сразу
            if (!card.Sections.Any())
            {
                return false;
            }

            // список отслеживаемых полей в виде названия секции + ключа поля
            Dictionary<string, List<string>> trackedSectionFields = new Dictionary<string, List<string>>()
            {
                {
                    "DocumentCommonInfo",
                    new List<string>()
                    {
                        "FullNumber" // внутренний номер
                    }
                },
                {
                    MainSectionName,
                    new List<string>()
                    {
                        "ExternalNumber", // Внешний номер
                        "CFOID", // ЦФО
                        "ProjectID", // Проект
                        "CostItemID" // Статья затрат
                    }
                }
            };

            // если хотя бы одно отслеживаемое поле содержится в карточке, значит оно было изменено
            foreach (var sectionFields in trackedSectionFields)
            {
                if (card.Sections.TryGetValue(sectionFields.Key, out var section)
                    && section.Fields.Any(field => sectionFields.Value.Contains(field.Key)))
                {
                    return true;
                }
            }

            return false;
        }

        protected override string MainSectionName => "PnrContracts";

        protected override bool IsNeedSendMessageToMdm(DateTime? lastSentDate)
        {
            // 1. было ли только что завершено согласование
            bool isApproveJustEnded =
                card.Tasks.Any(x =>
                    // тип задания "Подписание"
                    (x.TypeID == PnrTaskTypes.PnrTaskSigningTypeID || x.TypeID == PnrTaskTypes.PnrTaskSignedTypeID)
                    // задание создается (значит было завершено согласование)
                    && x.State == Cards.CardRowState.Inserted);

            // 2. была ли изменена карточка после согласования
            bool isCardChangedAfterApprove =
                // если дата задана, значит карточка была согласована ранее
                lastSentDate != null
                // изменилось хотя бы одно отслеживаемое поле
                && IsContractTrackedFieldsChanged(card);

            // передаем карточку в НСИ в любом из случаев:
            if (isApproveJustEnded
                || isCardChangedAfterApprove)
            {
                return true;
            }

            return false;
        }

        protected override async Task<object> GetMessageDataFromCardAsync(Card loadedCard)
        {
            PnrContractRequest result = new PnrContractRequest();

            if (loadedCard == null)
            {
                return null;
            }
            if (!loadedCard.Sections.TryGetValue(MainSectionName, out var mainSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"{MainSectionName}\" для карточки {loadedCard.ID}");
                return null;
            }
            if (!loadedCard.Sections.TryGetValue("DocumentCommonInfo", out var dciSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"DocumentCommonInfo\" для карточки {loadedCard.ID}");
                return null;
            }
            if (!loadedCard.Sections.TryGetValue("FdSatelliteCommonInfoVirtual", out var fscivSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"FdSatelliteCommonInfoVirtual\" для карточки {loadedCard.ID}");
                return null;
            }

            // Создал
            var authorID = dciSection.Fields.TryGet<Guid?>("AuthorID");
            dciSection.Fields.TryGet<string>("AuthorName");

            // Заявитель
            var applicant = await ServerHelper.GetPersonalRolesFields(dbScope, authorID);

            // Дата создания
            var creationDate = dciSection.Fields.TryGet<DateTime?>("CreationDate");

            // Номер
            string fullNumber = dciSection.Fields.TryGet<string>("FullNumber");

            // Дата заключения
            DateTime? projectDate = mainSection.Fields.TryGet<DateTime?>("ProjectDate");
            if (projectDate == null)
            {
                validationResult.AddError($"Не удалось определить значение поля \"Дата заключения\" для карточки {loadedCard.ID}");
                return null;
            }

            // Тип документа
            dciSection.Fields.TryGet<Guid?>("CardTypeID");
            dciSection.Fields.TryGet<string>("CardTypeCaption");

            // Статус
            fscivSection.Fields.TryGet<Guid?>("StateID");
            var stateName = fscivSection.Fields.TryGet<string>("StateName");

            // Внешний номер
            string externalNumber = mainSection.Fields.TryGet<string>("ExternalNumber");

            // Вид договора
            Guid? contractKindID = mainSection.Fields.TryGet<Guid?>("KindID");
            var contractKind = contractKindID != null
                ? await PnrContractKindHelper.GetContractKindByID(dbScope, contractKindID.Value)
                : null;
            // Нам нужно отправить "Название в НСИ (для интеграции)". Если оно не задано, отправим обычное название.
            string contractKindMdmName = contractKind?.MDMName ?? contractKind?.Name;

            // Предмет договора
            string subject = mainSection.Fields.TryGet<string>("Subject");

            // Тема
            dciSection.Fields.TryGet<string>("Subject");

            // Контрагент
            Guid? partnerID = mainSection.Fields.TryGet<Guid?>("PartnerID");
            // MDM-ключ контрагента
            string partnerMdmKey = partnerID != null
                ? (await PnrPartnerHelper.GetPartnerByID(dbScope, partnerID.Value))?.MDMKey
                : null;

            // Организация ГК Пионер
            Guid? organizationID = mainSection.Fields.TryGet<Guid?>("OrganizationID");
            // MDM-ключ организации
            string organizationMdmKey = organizationID != null
                ? (await PnrOrganizationHelper.GetOrganizationByID(dbScope, organizationID.Value))?.MDMKey
                : null;

            // Индекс ЮЛ
            mainSection.Fields.TryGet<Guid?>("LegalEntityIndexID");
            mainSection.Fields.TryGet<string>("LegalEntityIndexName");

            // ЦФО
            Guid? cfoID = mainSection.Fields.TryGet<Guid?>("CFOID");
            // MDM-ключ ЦФО
            string cfoMdmKey = cfoID != null
                ? (await PnrCFOHelper.GetCFOByID(dbScope, cfoID.Value))?.MDMKey
                : null;

            // Проект
            Guid? projectID = mainSection.Fields.TryGet<Guid?>("ProjectID");
            // MDM-ключ проекта
            string projectMdmKey = projectID != null
                ? (await PnrProjectHelper.GetProjectByID(dbScope, projectID.Value))?.MDMKey
                : null;

            // Наименование статьи затрат
            Guid? costItemID = mainSection.Fields.TryGet<Guid?>("CostItemID");
            // MDM-ключ статьи затрат
            string costItemMdmKey = costItemID != null
                ? (await PnrCostItemHelper.GetCostItemByID(dbScope, costItemID.Value))?.MDMKey
                : null;

            // Сумма договора (руб.)
            var amount = mainSection.Fields.TryGet<decimal?>("Amount");

            // Сумма аванса (руб.)
            var prepaidExpenseAmount = mainSection.Fields.TryGet<decimal?>("PrepaidExpenseAmount");

            // Валюта расчета
            Guid? settlementCurrencyID = mainSection.Fields.TryGet<Guid?>("SettlementCurrencyID");
            // MDM-ключ валюты
            string settlementCurrencyMdmKey = settlementCurrencyID != null
                ? (await PnrCurrencyHelper.GetCurrencyByID(dbScope, settlementCurrencyID.Value))?.MDMKey
                : null;

            // Ставка НДС
            var vatRateID = mainSection.Fields.TryGet<int?>("VATRateID");
            mainSection.Fields.TryGet<string>("VATRateValue");

            // значение НДС
            var vatValue = PnrVatRateHelper.GetVatRateValueByID(vatRateID);

            // В бюджете
            bool? isInBudget = mainSection.Fields.TryGet<bool?>("IsInBudget");

            // Проведен тендер
            mainSection.Fields.TryGet<bool?>("IsTenderHeld");

            // Отсрочка платежа (раб.дн.)
            mainSection.Fields.TryGet<int?>("DefermentPaymentID");
            mainSection.Fields.TryGet<string>("DefermentPaymentValue");

            // Планируемая дата актирования
            DateTime? plannedActDate = mainSection.Fields.TryGet<DateTime?>("PlannedActDate");

            // Форма договора
            mainSection.Fields.TryGet<Guid?>("FormID");
            mainSection.Fields.TryGet<string>("FormName");

            // Дата начала
            var startDate = mainSection.Fields.TryGet<DateTime?>("StartDate");

            // Дата окончания
            var endDate = mainSection.Fields.TryGet<DateTime?>("EndDate");

            // Тип договора
            mainSection.Fields.TryGet<Guid?>("TypeID");
            string contractTypeName = mainSection.Fields.TryGet<string>("TypeName");

            // Вид договора (1С)
            mainSection.Fields.TryGet<Guid?>("Kind1CID");
            mainSection.Fields.TryGet<string>("Kind1CName");

            // Комментарий
            string comment = mainSection.Fields.TryGet<string>("Comment");

            // Подписант
            mainSection.Fields.TryGet<Guid?>("SignatoryID");
            mainSection.Fields.TryGet<string>("SignatoryName");

            /* Договоры ДУП */

            // % авансового платежа
            mainSection.Fields.TryGet<int?>("DownPaymentID");
            string downPaymentValue = mainSection.Fields.TryGet<string>("DownPaymentValue");

            // Отсрочка платежа (раб.дн.) ДУП
            mainSection.Fields.TryGet<string>("DefermentPaymentDUP");

            // Условие начала выполнения работ
            string conditionStartingWorkName = mainSection.Fields.TryGet<string>("ConditionStartingWorkName");

            // Предусмотрено поэтапное выполнение
            int? phasedImplementationID = mainSection.Fields.TryGet<int?>("PhasedImplementationID");
            bool isPhasedImplementation = phasedImplementationID == 1;

            // Ссылка на карточку в толстом клиенте
            var cardLink = CardHelper.GetLink(session, card.ID);
            cardLink = cardLink.Replace("&amp", "&");

            result.ClassId = 425;
            result.CreationTime = DateTime.UtcNow.ToString("s");
            result.Id = Guid.NewGuid().ToString();
            result.NeedAcknowledgment = false;
            result.Receivers = "";
            result.Source = "CIS";
            result.Type = "DTP";
            result.CorrelationId = Guid.Empty.ToString();

            result.Body = new PnrContractRequestBody();
            result.Body.ContractData = new PnrContractRequestBodyContractData();
            var resultMain = result.Body.ContractData;

            resultMain.ПометкаУдаления = false;
            resultMain.ИдентификаторЗаявки = loadedCard.ID.ToString();
            resultMain.СистемаИсточник = "КИС";
            resultMain.Наименование = subject;
            resultMain.п_ТекущийСтатусВСЭД = stateName;
            resultMain.ВалютаРасчетов = settlementCurrencyMdmKey;
            resultMain.ДатаДоговора = projectDate.Value;
            resultMain.п_ДатаОкончания = endDate;
            resultMain.Контрагент = partnerMdmKey;
            // Если указан внешний номер, то передается он, если поле внешний номер пустое, то передается внутренний номер договора.
            resultMain.НомерДоговора = !string.IsNullOrWhiteSpace(externalNumber) ? externalNumber : fullNumber;
            resultMain.Организация = organizationMdmKey;
            resultMain.ОсновнаяСтатьяОборотов = costItemMdmKey;
            resultMain.ЦФО = cfoMdmKey;
            resultMain.СуммаДоговора = amount?.ToString();
            resultMain.Проект = projectMdmKey;
            resultMain.ВидДоговора = contractKindMdmName;
            resultMain.СсылкаНаКаталогФайла = cardLink;
            resultMain.Комментарий = comment;
            resultMain.п_ПроцентАванса = downPaymentValue;
            if (prepaidExpenseAmount != null && prepaidExpenseAmount > 0)
            {
                resultMain.п_Аванс = "true";
                resultMain.п_СуммаАванса = prepaidExpenseAmount?.ToString();
            }
            resultMain.п_СтавкаНДС = vatValue?.ToString();
            resultMain.п_ВБюджете = isInBudget;
            resultMain.ПредусмотреноПоэтапноеВыполнение = isPhasedImplementation;
            resultMain.п_ТипДоговора = contractTypeName;
            resultMain.п_ПричинаЗаключенияДС = conditionStartingWorkName;
            resultMain.п_СЭД_ДатаАктирования = plannedActDate;
            resultMain.п_ИдентификаторЗаявкиСЭД = loadedCard.ID.ToString();
            resultMain.Заявитель = new PnrContractRequestBodyContractDataЗаявитель()
            {
                Email = applicant.Email,
                ДоменноеИмя = applicant.Login,
                ПолноеИмя = applicant.FullName
            };

            return result;
        }
    }
}
