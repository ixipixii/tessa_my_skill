using System;
using System.Collections.Generic;
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
    public partial class PnrMdmUpdateContractRequest : PnrBaseRequest
    {
        public override Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return Task.FromResult(GetGuidFromString(this.Body?.classData?.п_ИдентификаторЗаявкиСЭД));
        }

        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope)
        {
            var cardID = await GetCardIDAsync(dbScope);
            return cardID != null ? await PnrCardHelper.GetCardTypeIDByCardID(dbScope, cardID.Value) : null;
        }

        public override string GetDescription()
        {
            return GetTrimmedString(this.Body?.classData?.НомерДоговора);
        }

        public override async Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult)
        {
            if (this.Body?.classData == null)
            {
                validationResult.AddError("Элемент \"classData\" не содержит значений.");
                return;
            }
            var data = this.Body?.classData;

            if (card.Sections.TryGetValue("DocumentCommonInfo", out var dciSection)
                &&
                // основная секция будет разной в зависимости от типа карточки
                card.TypeID == PnrCardTypes.PnrContractTypeID
                ? card.Sections.TryGetValue("PnrContracts", out var mainSection)
                : card.Sections.TryGetValue("PnrSupplementaryAgreements", out mainSection))
            {
                // все поля, пришедшие в запросе

                // MDM-key
                if (!string.IsNullOrEmpty(data.MDM_Key))
                {
                    mainSection.Fields["MDMKey"] = data.MDM_Key;
                }

                // Внешний номер
                if (!string.IsNullOrEmpty(data.НомерДоговора))
                {
                    mainSection.Fields["ExternalNumber"] = GetTrimmedString(data.НомерДоговора);
                }

                // Дата заключения
                if (data.ДатаДоговора != null)
                {
                    mainSection.Fields["ProjectDate"] = GetUtcDate(data.ДатаДоговора.Value);
                }

                // Валюта расчета
                if (!string.IsNullOrEmpty(data.ВалютаРасчетов))
                {
                    var currency = await PnrCurrencyHelper.GetCurrencyByMDM(dbScope, data.ВалютаРасчетов);
                    if (currency != null)
                    {
                        mainSection.Fields["SettlementCurrencyID"] = currency.ID;
                        mainSection.Fields["SettlementCurrencyName"] = currency.Name;
                        mainSection.Fields["SettlementCurrencyCode"] = currency.Code;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти валюту расчета. MDM-Key={data.ВалютаРасчетов}.");
                    }
                }

                // ЦФО
                if (!string.IsNullOrEmpty(data.ЦФО))
                {
                    var cfo = await PnrCFOHelper.GetCFOByMDM(dbScope, data.ЦФО);
                    if (cfo != null)
                    {
                        mainSection.Fields["CFOID"] = cfo.ID;
                        mainSection.Fields["CFOName"] = cfo.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти ЦФО. MDM-Key={data.ЦФО}.");
                    }
                }

                // Проект
                if (!string.IsNullOrEmpty(data.Проект))
                {
                    var project = await PnrProjectHelper.GetProjectByMDM(dbScope, data.Проект);
                    if (project != null)
                    {
                        mainSection.Fields["ProjectID"] = project.ID;
                        mainSection.Fields["ProjectName"] = project.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти проект. MDM-Key={data.Проект}.");
                    }
                }

                // Статья затрат
                if (!string.IsNullOrEmpty(data.ОсновнаяСтатьяОборотов))
                {
                    var costItem = await PnrCostItemHelper.GetCostItemByMDM(dbScope, data.ОсновнаяСтатьяОборотов);
                    if (costItem != null)
                    {
                        mainSection.Fields["CostItemID"] = costItem.ID;
                        mainSection.Fields["CostItemName"] = costItem.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти статью затрат. MDM-Key={data.ОсновнаяСтатьяОборотов}.");
                    }
                }
            }
        }
    }
}
