using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    public partial class PnrCFORequest : PnrBaseRequest
    {
        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope) => PnrCardTypes.PnrCFOTypeID;

        public override async Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return await TryGetCardIDFromMdmKeyAsync(dbScope, this.Body?.classData?.MDM_Key, "PnrCFO");
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
            var cfo = this.Body?.classData;

            if (card.Sections.TryGetValue("PnrCFO", out var mainSection))
            {
                // MDM-Key
                mainSection.Fields["MDMKey"] = cfo.MDM_Key;

                // Код
                mainSection.Fields["Code"] = GetTrimmedString(cfo.Код);

                // Название
                mainSection.Fields["Name"] = GetTrimmedString(cfo.Наименование);

                // Вид ЦФО
                mainSection.Fields["Type"] = GetTrimmedString(cfo.ВидЦФО);

                // Описание
                mainSection.Fields["Description"] = GetTrimmedString(cfo.Комментарий);

                // Используется
                mainSection.Fields["Used"] = GetTrimmedString(cfo.Используется);

                // Родительский ЦФО
                if (!string.IsNullOrEmpty(cfo.Родитель))
                {
                    var parentCFO = await PnrCFOHelper.GetCFOByMDM(dbScope, cfo.Родитель);
                    if (parentCFO != null)
                    {
                        mainSection.Fields["ParentCFOID"] = parentCFO.ID;
                        mainSection.Fields["ParentCFOName"] = parentCFO.Name;
                    }
                    else
                    {
                        validationResult.AddError($"Не удалось найти ЦФО. MDM-Key={cfo.Родитель}.");
                    }
                }
            }
        }
    }
}
