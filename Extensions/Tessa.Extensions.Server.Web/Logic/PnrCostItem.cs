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
    public partial class PnrCostItemRequest : PnrBaseRequest
    {
        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope) => PnrCardTypes.PnrCostItemTypeID;

        public override async Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return await TryGetCardIDFromMdmKeyAsync(dbScope, this.Body?.classData?.MDM_Key, "PnrCostItems");
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
            var costItem = this.Body?.classData;

            if (card.Sections.TryGetValue("PnrCostItems", out var mainSection))
            {
                // MDM-Key
                mainSection.Fields["MDMKey"] = costItem.MDM_Key;

                // Код
                mainSection.Fields["Code"] = GetTrimmedString(costItem.Код);

                // Наименование
                mainSection.Fields["Name"] = GetTrimmedString(costItem.Наименование);

                // Это группа
                mainSection.Fields["IsGroup"] = GetStringFromBool(costItem.ЭтоГруппа);

                // Кодификатор
                mainSection.Fields["Codifier"] = GetTrimmedString(costItem.Кодификатор);

                // Используется
                mainSection.Fields["Used"] = GetTrimmedString(costItem.Используется);

                // Расход/Доход
                mainSection.Fields["ConsumptionIncome"] = GetTrimmedString(costItem.РасходДоход);

                // Тип статьи
                mainSection.Fields["ItemType"] = GetTrimmedString(costItem.ТипСтатьи);

                // Идентификатор Казна
                mainSection.Fields["IdentifierTreasury"] = GetTrimmedString(costItem.ИдентификаторКазна);

                // Удалено
                mainSection.Fields["IsRemoved"] = costItem.ПометкаУдаления;

                // Описание
                // mainSection.Fields["Description"] = ;

                // Родительская статья затрат
                if (!string.IsNullOrEmpty(costItem.Родитель))
                {
                    var parentCostItem = await PnrCostItemHelper.GetCostItemByMDM(dbScope, costItem.Родитель);
                    if (parentCostItem != null)
                    {
                        mainSection.Fields["ParentСodifierID"] = parentCostItem.ID;
                        mainSection.Fields["ParentСodifierName"] = parentCostItem.Name;
                    }
                    else
                    {
                        validationResult.AddError($"Не удалось найти статью затрат. MDM-Key={costItem.Родитель}.");
                    }
                }
            }
        }
    }
}
