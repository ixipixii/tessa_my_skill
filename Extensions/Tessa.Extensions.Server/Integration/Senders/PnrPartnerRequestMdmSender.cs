using LinqToDB.Common;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Server.Integration.Models;
using Tessa.Extensions.Shared.Models;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Senders
{
    public class PnrPartnerRequestMdmSender : PnrMdmBaseSender
    {
        public PnrPartnerRequestMdmSender(IDbScope dbScope, ICardRepository cardRepository, ICardCache cardCache, IValidationResultBuilder validationResult, ILogger logger, Card card, ISession session) : base(dbScope, cardRepository, cardCache, validationResult, logger, card, session)
        {
        }

        private static bool GetPartnerMdmValueFromTypeID(int? partnerTypeID, out string mdmPartnerType)
        {
            mdmPartnerType = null;

            if (partnerTypeID == null)
            {
                return false;
            }

            // ЮЛ
            if (partnerTypeID == 1)
            {
                mdmPartnerType = "ЮрЛицо";
            }
            // ФЛ
            else if (partnerTypeID == 2)
            {
                mdmPartnerType = "ФизЛицо";
            }
            // ИП
            else if (partnerTypeID == 3)
            {
                mdmPartnerType = "аксИП";
            }

            return mdmPartnerType != null;
        }

        protected override string MainSectionName => "PnrPartnerRequests";

        protected override bool IsNeedSendMessageToMdm(DateTime? lastSentDate)
        {
            if (card.Tasks.Any(x =>
                // тип задания "Отправка заявки контрагента"
                x.TypeID == PnrTaskTypes.PnrPartnerRequestStartIntegrationTypeID
                // задание завершается
                && x.Action == CardTaskAction.Complete
                && x.State == Cards.CardRowState.Deleted
                // кнопка "Отправить"
                && x.OptionID == DefaultCompletionOptions.SendToPerformer))
            {
                return true;
            }

            return false;
        }

        protected override async Task<object> GetMessageDataFromCardAsync(Card loadedCard)
        {
            PnrReqOnPartnerRequest result = new PnrReqOnPartnerRequest();

            if (loadedCard == null)
            {
                return null;
            }
            if (!loadedCard.Sections.TryGetValue("PnrPartnerRequests", out var mainSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"PnrPartnerRequests\" для карточки {loadedCard.ID}");
                return null;
            }
            if (!loadedCard.Sections.TryGetValue("DocumentCommonInfo", out var dciSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"DocumentCommonInfo\" для карточки {loadedCard.ID}");
                return null;
            }
            //if (!loadedCard.Sections.TryGetValue("FdSatelliteCommonInfoVirtual", out var fscivSection))
            //{
            //    validationResult.AddError($"Не удалось найти секцию \"FdSatelliteCommonInfoVirtual\" для карточки {loadedCard.ID}");
            //    return null;
            //}

            // Создал
            var authorID = dciSection.Fields.TryGet<Guid?>("AuthorID");
            dciSection.Fields.TryGet<string>("AuthorName");

            // Заявитель
            var applicant = await ServerHelper.GetPersonalRolesFields(dbScope, authorID);

            if (applicant == null
                || string.IsNullOrWhiteSpace(applicant.Email))
            {
                validationResult.AddError($"Не удалось определить Email заявителя!");
                return null;
            }

            // Дата создания
            dciSection.Fields.TryGet<DateTime?>("CreationDate");

            // Номер
            dciSection.Fields.TryGet<string>("FullNumber");

            // Дата регистрации
            mainSection.Fields.TryGet<DateTime?>("RegistrationDate");

            // Тип документа
            dciSection.Fields.TryGet<Guid?>("CardTypeID");
            dciSection.Fields.TryGet<string>("CardTypeCaption");

            // Тип заявки
            var requestTypeID = mainSection.Fields.TryGet<int?>("RequestTypeID");
            mainSection.Fields.TryGet<string>("RequestTypeName");

            // Рег. №
            mainSection.Fields.TryGet<string>("RegistrationNo");

            // Тип контрагента
            int? typeID = mainSection.Fields.TryGet<int?>("TypeID");
            string typeName = mainSection.Fields.TryGet<string>("TypeName");

            // Контрагент
            var partnerID = mainSection.Fields.TryGet<Guid?>("PartnerID");
            var partnerName = mainSection.Fields.TryGet<string>("PartnerName");

            // Наименование
            mainSection.Fields.TryGet<string>("Name");

            // Полное наименование контрагента
            string fullName = mainSection.Fields.TryGet<string>("FullName");

            // Краткое наименование контагента
            string shortName = mainSection.Fields.TryGet<string>("ShortName");

            // Особый признак контрагента
            mainSection.Fields.TryGet<int?>("SpecialSignID");
            mainSection.Fields.TryGet<string>("SpecialSignName");

            // Нерезидент
            mainSection.Fields.TryGet<int?>("NonResidentID");
            mainSection.Fields.TryGet<string>("NonResidentName");

            // ИНН
            string inn = mainSection.Fields.TryGet<string>("INN");

            // КПП
            string kpp = mainSection.Fields.TryGet<string>("KPP");

            // ОГРН
            string ogrn = mainSection.Fields.TryGet<string>("OGRN");

            // Паспортные данные
            mainSection.Fields.TryGet<string>("Passport");

            // День рождения
            DateTime? birthday = mainSection.Fields.TryGet<DateTime?>("Birthday");

            // Страна регистрации
            Guid? countryRegistrationID = mainSection.Fields.TryGet<Guid?>("CountryRegistrationID");
            mainSection.Fields.TryGet<string>("CountryRegistrationName");

            // Комментарий
            string comment = mainSection.Fields.TryGet<string>("Comment");

            // Требует согласование КА
            mainSection.Fields.TryGet<bool?>("RequiresApprovalCA");

            result.ClassId = 424;
            result.CreationTime = DateTime.UtcNow.ToString("s");
            result.Id = Guid.NewGuid().ToString();
            result.NeedAcknowledgment = false;
            result.Receivers = "";
            result.Source = "CIS";
            result.Type = "DTP";
            result.CorrelationId = Guid.Empty.ToString();

            result.Body = new PnrReqOnPartnerRequestBody();
            result.Body.ContractRequestData = new PnrReqOnPartnerRequestBodyContractRequestData();
            var resultMain = result.Body.ContractRequestData;

            resultMain.ПометкаУдаления = false;
            resultMain.СистемаИсточник = "КИС";
            resultMain.Наименование = shortName;
            resultMain.ИНН = inn;
            resultMain.КПП = kpp;
            resultMain.ОГРН = ogrn;
            resultMain.Комментарий = comment;
            resultMain.ПолноеНаименование = fullName;
            if (GetPartnerMdmValueFromTypeID(typeID, out string mdmPartnerType))
            {
                resultMain.ЮрФизЛицо = mdmPartnerType;
            }

            // страна регистрации
            string countryMdm = null;
            if (countryRegistrationID != null)
            {
                var country = await PnrCountryHelper.GetCountryByID(dbScope, countryRegistrationID.Value);
                if (country != null
                    && !string.IsNullOrEmpty(country.MDMKey))
                {
                    countryMdm = country.MDMKey;
                }
            }

            // Если страна не указана, то берем Россию
            resultMain.СтранаРегистрации = countryMdm ?? "17b6bf39-5918-11e8-8134-005056a71cd1";

            resultMain.ДатаРождения = birthday?.ToString();

            resultMain.Заявитель = new PnrReqOnPartnerRequestBodyContractRequestDataЗаявитель()
            {
                Email = applicant.Email,
                ДоменноеИмя = "\\\\" + applicant.Login,
                ПолноеИмя = applicant.FullName
            };

            //// первоначальная заявка на создание
            //PnrPartnerRequest createPartnerRequest = null;

            // если это заявка на создание нового КА
            if (requestTypeID == 0)
            {
                // передадим состояние
                resultMain.п_Статус = PnrPartnersStatus.NotAgreedName;
            }

            // если КА задан и это заявка на согласование
            if (partnerID != null && requestTypeID == 1)
            {
                // загрузим КА
                var partnerData = await PnrPartnerHelper.GetPartnerByID(dbScope, partnerID.Value);

                if (partnerData == null)
                {
                    validationResult.AddError($"Не удалось загрузить информацию контрагенту {partnerID.Value}");
                    return null;
                }

                //// загрузим связанные с КА заявки
                //var partnerRequests = await PnrPartnerRequestHelper.GetPartnerRequestsByPartnerID(dbScope, partnerID.Value);

                //// ищем заявку на создание
                //createPartnerRequest = partnerRequests.FirstOrDefault(x => x.RequestTypeID == 0 && x.ID != card.ID);

                // в MDM передадим ключ КА
                resultMain.MDM_Key = partnerData.MDMKey?.ToString();

                // передадим состояние и дату получения статуса
                resultMain.п_Статус = partnerData.StatusName;
                resultMain.п_ДатаУстановкиСтатуса = partnerData.DateApproval;
            }

            resultMain.ИдентификаторЗаявки = loadedCard.ID.ToString();
            resultMain.п_ИдентификаторЗаявкиСЭД = loadedCard.ID.ToString();

            return result;
        }
    }
}
