using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Web.Models;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    public partial class PnrOrganizationRequest : PnrBaseRequest
    {
        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope) => PnrCardTypes.PnrOrganizationTypeID;

        public override async Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return await TryGetCardIDFromMdmKeyAsync(dbScope, this.Body?.classData?.MDM_Key, "PnrOrganizations");
        }

        public override string GetDescription()
        {
            return GetTrimmedString(this.Body?.classData?.Наименование);
        }

        public override async Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult)
        {
            if (this.Body?.classData == null)
            {
                validationResult.AddError("Элемент \"classData\" не содержит значений.");
                return;
            }
            var organization = this.Body?.classData;

            if (card.Sections.TryGetValue("PnrOrganizations", out var mainSection))
            {
                // MDM-Key
                mainSection.Fields["MDMKey"] = organization.MDM_Key;

                // Наименование
                mainSection.Fields["Name"] = GetTrimmedString(organization.Наименование);

                // Сокращенное наименование
                mainSection.Fields["ShortName"] = GetTrimmedString(organization.аксСокращенноеНаименование);

                // Полное наименование
                mainSection.Fields["FullName"] = GetTrimmedString(organization.ПолноеНаименование);

                // Аббревиатура
                mainSection.Fields["Abbreviation"] = GetTrimmedString(organization.Префикс);

                // Код
                //mainSection.Fields["Code"] = organization.Код;

                // Префикс
                mainSection.Fields["Prefix"] = GetTrimmedString(organization.Префикс);

                // Контрагент
                if (!string.IsNullOrEmpty(organization.Контрагент))
                {
                    var partner = await PnrPartnerHelper.GetPartnerByMDM(dbScope, organization.Контрагент);
                    if (partner != null)
                    {
                        mainSection.Fields["PartnerID"] = partner.ID;
                        mainSection.Fields["PartnerName"] = partner.Name;
                    }
                    else
                    {
                        // разрешаем сохранение
                        validationResult.AddWarning($"Не удалось найти контрагента. MDM-Key={organization.Контрагент}.");
                    }
                }

                // ИНН
                mainSection.Fields["INN"] = GetTrimmedString(organization.ИНН);

                // КПП
                mainSection.Fields["KPP"] = GetTrimmedString(organization.КПП);

                var contactInfo = organization.КонтактнаяИнформация?.FirstOrDefault();

                // Адрес
                mainSection.Fields["Address"] = GetTrimmedString(contactInfo?.Представление);

                // ОГРН
                mainSection.Fields["OGRN"] = GetTrimmedString(organization.ОГРН);

                // Рабочий телефон
                mainSection.Fields["WorkPhone"] = GetTrimmedString(contactInfo?.НомерТелефона);

                // Сайт
                mainSection.Fields["Website"] = GetTrimmedString(contactInfo?.ДоменноеИмяСервера);

                // Код по ОКПО
                mainSection.Fields["OKPOCode"] = GetTrimmedString(organization.КодПоОКПО);

                // Дата выдачи
                mainSection.Fields["IssueDate"] = GetDateFromString(organization.аксДатаВыдачи);

                // Дата регистрации ЮЛ ГКП
                mainSection.Fields["RegistrationDate"] = organization.аксДатаРегиистрации != null ? GetUtcDate(organization.аксДатаРегиистрации.Value) : (DateTime?) null;

                // Иностранная организация
                mainSection.Fields["ForeignOrganization"] = GetStringFromBool(organization.аксИностраннаяОрганизация);

                // Наименование иностранной организации
                mainSection.Fields["ForeignOrganizationName"] = GetTrimmedString(organization.аксНаименованиеИностраннойОрганизации);

                // Код налогового органа
                mainSection.Fields["TaxAuthorityCode"] = GetTrimmedString(organization.аксКодНалоговогоОргана);

                // Наименование налогового органа
                mainSection.Fields["TaxAuthorityName"] = GetTrimmedString(organization.аксНаименованиеНалоговогоОргана);

                // Код ОКВЭД
                mainSection.Fields["OKVEDCode"] = GetTrimmedString(organization.аксКодОКВЭД);

                // Наименование ОКВЭД
                mainSection.Fields["OKVEDName"] = GetTrimmedString(organization.аксНаименованиеОКВЭД);

                // Код ОКВЭД 2
                mainSection.Fields["OKVEDCode2"] = GetTrimmedString(organization.аксКодОКВЭД2);

                // Наименование ОКВЭД 2
                mainSection.Fields["OKVEDName2"] = GetTrimmedString(organization.аксНаименованиеОКВЭД2);

                // Код ОКОПФ
                mainSection.Fields["OKOPFCode"] = GetTrimmedString(organization.аксКодОКОПФ);

                // Наименование ОКОПФ
                mainSection.Fields["OKOPFName"] = GetTrimmedString(organization.аксНаименованиеОКОПФ);

                // Код ОКФС
                mainSection.Fields["OKFSCode"] = GetTrimmedString(organization.аксКодОКФС);

                // Наименование ОКФС
                mainSection.Fields["OKFSName"] = GetTrimmedString(organization.аксНаименованиеОКФС);

                // Код органа ФСГС
                mainSection.Fields["AuthorityFSGSCode"] = GetTrimmedString(organization.аксКодОрганаФСГС);

                // Наименование территориального органа ФСС
                mainSection.Fields["TerritorialAuthorityFSSName"] = GetTrimmedString(organization.аксНаименованиеТерриториальногоОрганаФСС);

                // Свидетельство Код органа
                mainSection.Fields["CertificateAuthorityCode"] = GetTrimmedString(organization.аксСвидетельствоКодОргана);

                // Серия номер свидетельства
                mainSection.Fields["CertificateSeriesNumber"] = GetTrimmedString(organization.аксСерияНомерСвидетельства);

                // Свидетельство Наименование органа
                mainSection.Fields["CertificateAuthorityName"] = GetTrimmedString(organization.аксСвидетельствоНаименованиеОргана);

                // Регистрационный номер ФСС
                mainSection.Fields["RegistrationNumberFSS"] = GetTrimmedString(organization.аксРегистрационныйНомерФСС);

                // Код подчиненности ФСС
                mainSection.Fields["SubordinationCodeFSS"] = GetTrimmedString(organization.аксКодПодчиненностиФСС);

                // Страна постоянного местонахождения
                if (!string.IsNullOrEmpty(organization.аксСтранаПостоянногоМестонахождения))
                {
                    var countryPermanentResidence = await PnrCountryHelper.GetCountryByMDM(dbScope, organization.аксСтранаПостоянногоМестонахождения);
                    if (countryPermanentResidence != null)
                    {
                        mainSection.Fields["CountryPermanentResidenceID"] = countryPermanentResidence.ID;
                        mainSection.Fields["CountryPermanentResidenceName"] = countryPermanentResidence.Name;
                    }
                    else
                    {
                        // разрешаем сохранение
                        validationResult.AddWarning($"Не удалось найти страну постоянного местонахождения. MDM-Key={organization.аксСтранаПостоянногоМестонахождения}.");
                    }
                }

                // Страна регистрации
                if (!string.IsNullOrEmpty(organization.аксСтранаРегистрации))
                {
                    var countryRegistration = await PnrCountryHelper.GetCountryByMDM(dbScope, organization.аксСтранаРегистрации);
                    if (countryRegistration != null)
                    {
                        mainSection.Fields["CountryRegistrationID"] = countryRegistration.ID;
                        mainSection.Fields["CountryRegistrationName"] = countryRegistration.Name;
                    }
                    else
                    {
                        // разрешаем сохранение
                        validationResult.AddWarning($"Не удалось найти страну регистрации. MDM-Key={organization.аксСтранаРегистрации}.");
                    }
                }

                // Удалено
                mainSection.Fields["Removed"] = GetStringFromBool(organization.ПометкаУдаления);

                // Эталонная позиция
                mainSection.Fields["ReferencePosition"] = GetTrimmedString(organization.ЭталоннаяПозиция);

                // Контактная информация
                mainSection.Fields["ContactInformation"] = GetTrimmedString(contactInfo?.Представление);
            }
        }

    }
}
