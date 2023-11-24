using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Web.Models;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    public partial class PnrServicePartnerRequest : PnrBaseRequest
    {
        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope) => DefaultCardTypes.PartnerTypeID;

        public override async Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return await TryGetCardIDFromMdmKeyAsync(dbScope, this.Body?.classData?.MDM_Key, "Partners");
        }

        public override string GetDescription()
        {
            return GetTrimmedString(this.Body?.classData?.Наименование);
        }

        private bool GetPartnerTypeFromMDMValue(string mdmPartnerType, out int partnerTypeID, out string partnerTypeName)
        {
            partnerTypeID = 0;
            partnerTypeName = null;

            if (string.IsNullOrEmpty(mdmPartnerType))
            {
                return false;
            }

            // ЮрЛицо
            if (mdmPartnerType.Contains("Юр", StringComparison.CurrentCultureIgnoreCase))
            {
                partnerTypeID = 1;
                partnerTypeName = "Юридическое лицо";
                return true;
            }
            // ФизЛицо
            else if (mdmPartnerType.Contains("Физ", StringComparison.CurrentCultureIgnoreCase))
            {
                partnerTypeID = 2;
                partnerTypeName = "Физическое лицо";
                return true;
            }
            // аксИП
            else if (mdmPartnerType.Contains("ИП", StringComparison.CurrentCultureIgnoreCase))
            {
                partnerTypeID = 3;
                partnerTypeName = "Индивидуальный предприниматель";
                return true;
            }

            return false;
        }

        public override async Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult)
        {
            if (this.Body?.classData == null)
            {
                validationResult.AddError("Элемент \"classData\" не содержит значений.");
                return;
            }
            var partner = this.Body?.classData;

            if (card.Sections.TryGetValue("Partners", out var mainSection))
            {
                // MDM-Key
                mainSection.Fields["MDMKey"] = partner.MDM_Key;

                // Краткое наименование
                mainSection.Fields["Name"] = GetTrimmedString(partner.Наименование);

                // Тип контрагента
                if (GetPartnerTypeFromMDMValue(partner.ЮрФизЛицо, out var partnerTypeID, out var partnerTypeName))
                {
                    mainSection.Fields["TypeID"] = partnerTypeID;
                    mainSection.Fields["TypeName"] = partnerTypeName;
                }

                // Дата создания
                mainSection.Fields["DateCreation"] = DateTime.UtcNow;

                // Срок действия
                //mainSection.Fields["Validity"] = ;

                // Дата одобрения СБ
                //mainSection.Fields["DateApproval"] = ;

                // Статус
                //mainSection.Fields["Status"] = ;

                // Полное наименование
                mainSection.Fields["FullName"] = GetTrimmedString(partner.ПолноеНаименование);

                // Руководитель
                //mainSection.Fields["Head"] = ;

                // Главный бухгалтер
                //mainSection.Fields["ChiefAccountant"] = ;

                // Контактное лицо
                //mainSection.Fields["ContactPerson"] = ;

                // Адрес
                mainSection.Fields["ContactAddress"] = GetTrimmedString(partner.КонтактнаяИнформация?.FirstOrDefault()?.Представление);

                // Телефон
                mainSection.Fields["Phone"] = GetTrimmedString(partner.КонтактнаяИнформация?.FirstOrDefault()?.НомерТелефона);

                // Email
                mainSection.Fields["Email"] = GetTrimmedString(partner.КонтактнаяИнформация?.FirstOrDefault()?.АдресЭП);

                // Почтовый адрес (Юридический адрес)
                //mainSection.Fields["LegalAddress"] = ;

                // ИНН
                mainSection.Fields["INN"] = GetTrimmedString(partner.ИНН);

                // КПП
                mainSection.Fields["KPP"] = GetTrimmedString(partner.КПП);

                // ОГРН
                mainSection.Fields["OGRN"] = GetTrimmedString(partner.ОГРН);

                // ОКПО
                mainSection.Fields["OKPO"] = GetTrimmedString(partner.КодПоОКПО);

                // ОКВЭД
                //mainSection.Fields["OKVED"] = ;

                // Нерезидент
                mainSection.Fields["NonResident"] = partner.Нерезидент;

                // Документ, удостоверяющий личность
                mainSection.Fields["IdentityDocument"] = GetTrimmedString(partner.ДокументУдостоверяющийЛичность);

                // Документ. Вид документа.
                mainSection.Fields["IdentityDocumentKind"] = GetTrimmedString(partner.аксВидДокумента);

                // Документ. Серия.
                mainSection.Fields["IdentityDocumentSeries"] = GetTrimmedString(partner.Серия);

                // Документ. Номер.
                mainSection.Fields["IdentityDocumentNumber"] = GetTrimmedString(partner.Номер);

                // Документ. Кем выдан.
                mainSection.Fields["IdentityDocumentIssuedBy"] = GetTrimmedString(partner.КемВыдан);

                // Документ. Когда выдан.
                mainSection.Fields["IdentityDocumentIssueDate"] = GetDateFromString(partner.КогдаВыдан);

                // Заявки на создание
                //mainSection.Fields["CreationRequests"] = ;

                // Заявки на согласование
                //mainSection.Fields["CoordinationRequests"] = ;

                // Комментарий
                mainSection.Fields["Comment"] = GetTrimmedString(partner.Комментарий);

                // Страна регистрации
                if (!string.IsNullOrEmpty(partner.аксСтранаРегистрации))
                {
                    var country = await PnrCountryHelper.GetCountryByMDM(dbScope, partner.аксСтранаРегистрации);
                    if (country != null)
                    {
                        mainSection.Fields["CountryRegistrationID"] = country.ID;
                        mainSection.Fields["CountryRegistrationName"] = country.Name;
                    }
                    else
                    {
                        // разрешаем сохранение
                        validationResult.AddWarning($"Не удалось найти страну регистрации. MDM-Key={partner.аксСтранаРегистрации}.");
                    }
                }

                // статус "Не согласовано" ставим по умолчанию только при создании (иначе он будет перезатираться после согласования заявки (PnrSetPartnerStatusTaskExtension) при ответном сообщении из НСИ)
                if (card.StoreMode == CardStoreMode.Insert)
                {
                    mainSection.Fields["StatusID"] = PnrPartnersStatus.NotAgreedID;
                    mainSection.Fields["StatusName"] = PnrPartnersStatus.NotAgreedName;
                }
            }
        }

        public override async Task AfterCardStoreActionAsync(Logger logger, IDbScope dbScope, ICardRepository cardRepository, Card partnerCard, IValidationResultBuilder validationResult)
        {
            if (!partnerCard.Sections.TryGetValue("Partners", out var mainSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"Partners\"");
                return;
            }

            if (this.Body?.classData == null)
            {
                validationResult.AddError("Элемент \"classData\" не содержит значений.");
                return;
            }
            var partner = this.Body?.classData;

            // ID заявки КА
            var requestCardID = GetGuidFromString(partner.ИдентификаторЗаявкиСЭД);

            if (requestCardID == null)
            {
                return;
            }

            // загрузим заявку
            var getPartnerRequestResponse = await cardRepository.GetAsync(new CardGetRequest()
            {
                CardID = requestCardID.Value,
                CardTypeID = PnrCardTypes.PnrPartnerRequestTypeID
            });

            if (!getPartnerRequestResponse.ValidationResult.IsSuccessful())
            {
                validationResult.AddError($"Не удалось загрузить указанную заявку на контрагента, ID: {requestCardID.Value}");
                validationResult.Add(getPartnerRequestResponse.ValidationResult.Build());
                return;
            }

            var partnerRequestCard = getPartnerRequestResponse.Card;

            if (!partnerRequestCard.Sections.TryGetValue("PnrPartnerRequests", out var pprSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"PnrPartnerRequests\"");
                return;
            }
            if (!partnerRequestCard.Sections.TryGetValue("DocumentCommonInfo", out var dciSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"DocumentCommonInfo\"");
                return;
            }

            // тип заявки
            var requestTypeID = pprSection.Fields.TryGet<int?>("RequestTypeID");

            // требуется согласование КА
            var requiresApprovalCA = pprSection.Fields.TryGet<bool?>("RequiresApprovalCA");

            // автор заявки на создание
            var requestAuthorID = dciSection.Fields.TryGet<Guid?>("AuthorID") ?? partnerRequestCard.CreatedByID;
            var requestAuthorName = dciSection.Fields.TryGet<string>("AuthorName") ?? partnerRequestCard.CreatedByName;

            // если это заявка на создание
            if (requestTypeID == 0)
            {
                // значение КА в карточке
                var refPartnerID = pprSection.Fields.TryGet<Guid?>("PartnerID");

                if (refPartnerID != null)
                {
                    if (refPartnerID != partnerCard.ID)
                    {
                        logger.Warn($"Указанная заявка: {requestCardID} уже связана с контрагентом: {refPartnerID}");
                    }
                    else
                    {
                        logger.Info($"Указанная заявка: {requestCardID} уже связана с данным контрагентом.");
                    }
                }
                // КА не задан, в заявке надо проставить связь с текущим КА
                else
                {
                    pprSection.Fields["PartnerID"] = partnerCard.ID;
                    pprSection.Fields["PartnerName"] = mainSection.Fields.TryGet<string>("Name");

                    partnerRequestCard.RemoveAllButChanged();

                    // обновим заявку на создание
                    var partnerRequestStoreResponse = await cardRepository.StoreAsync(new CardStoreRequest()
                    {
                        Card = partnerRequestCard,
                        // передадим флаг, по которому будем пропускать проверку заполненности некоторых полей
                        Info = new Dictionary<string, object>()
                        {
                            { PnrInfoKeys.PnrSkipCardFieldsCustomValidation, true }
                        }
                    });

                    if (!partnerRequestStoreResponse.ValidationResult.IsSuccessful())
                    {
                        validationResult.AddError($"Не удалось сохранить указанную заявку на создание контрагента, ID: {requestCardID.Value}");
                        validationResult.Add(partnerRequestStoreResponse.ValidationResult.Build());
                        return;
                    }

                    logger.Info($"Заявка: {requestCardID} успешно привязана к контрагенту: {refPartnerID}");

                    // требуется согласование КА
                    if (requiresApprovalCA == true)
                    {
                        CardNewResponse partnerRequestNewResponse;

                        // создавать новую заявку на согласование будем под именем автора заявки на создание
                        using (SessionContext.Create(new SessionToken(requestAuthorID, requestAuthorName)))
                        {
                            // создадим заявку на согласование
                            partnerRequestNewResponse = await cardRepository.NewAsync(new CardNewRequest()
                            {
                                CardTypeID = PnrCardTypes.PnrPartnerRequestTypeID,
                                Info = new Dictionary<string, object>
                                {
                                    { PnrInfoKeys.PnrCreatePartnerRequestPartnerID, partnerCard.ID },
                                }
                            });
                        }

                        if (!partnerRequestNewResponse.ValidationResult.IsSuccessful())
                        {
                            validationResult.AddError($"Не удалось создать заявку на согласование контрагента.");
                            validationResult.Add(partnerRequestNewResponse.ValidationResult.Build());
                            return;
                        }

                        var newPartnerRequestCard = partnerRequestNewResponse.Card;
                        newPartnerRequestCard.ID = Guid.NewGuid();


                        // сохраним заявку на согласование
                        var newPartnerRequestStoreResponse = await cardRepository.StoreAsync(new CardStoreRequest()
                        {
                            Card = newPartnerRequestCard,
                            // передадим флаг, по которому будем пропускать проверку заполненности некоторых полей
                            Info = new Dictionary<string, object>()
                            {
                                { PnrInfoKeys.PnrSkipCardFieldsCustomValidation, true }
                            }
                        });

                        if (!newPartnerRequestStoreResponse.ValidationResult.IsSuccessful())
                        {
                            validationResult.AddError($"Не удалось сохранить заявку на согласование контрагента.");
                            validationResult.Add(newPartnerRequestStoreResponse.ValidationResult.Build());
                            return;
                        }

                        logger.Info($"Заявка на согласование: {requestCardID} успешно создана и привязана к контрагенту: {refPartnerID}");
                    }
                }

            }

            return;
        }
    }
}
