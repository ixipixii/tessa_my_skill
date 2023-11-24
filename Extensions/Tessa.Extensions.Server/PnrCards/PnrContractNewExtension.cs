using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrContractNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            object kindID, kindName, kindDUPID, kindDUPName;
            context.Request.Info.TryGetValue("KindID", out kindID);
            context.Request.Info.TryGetValue("KindName", out kindName);
            context.Request.Info.TryGetValue("KindDUPID", out kindDUPID);
            context.Request.Info.TryGetValue("KindDUPName", out kindDUPName);

            if (kindID != null)
            {
                card.Sections["PnrContracts"].Fields["KindID"] = (Guid)kindID;
                card.Sections["PnrContracts"].Fields["KindName"] = (string)kindName;
                card.Permissions.Sections.GetOrAdd("PnrContracts").FieldPermissions["KindID"] = CardPermissionFlags.ProhibitModify;
            }

            if (kindDUPID != null)
            {
                card.Sections["PnrContracts"].Fields["KindDUPID"] = (Guid)kindDUPID;
                card.Sections["PnrContracts"].Fields["KindDUPName"] = (string)kindDUPName;
                card.Permissions.Sections.GetOrAdd("PnrContracts").FieldPermissions["KindDUPID"] = CardPermissionFlags.ProhibitModify;
            }

            // Вид договора (1С)
            card.Sections["PnrContracts"].Fields["Kind1CID"] = PnrContractKinds.PnrContract1CWithSupplierID;
            card.Sections["PnrContracts"].Fields["Kind1CName"] = PnrContractKinds.PnrContract1CWithSupplierName;

            // Валюта (RUB)
            card.Sections["PnrContracts"].Fields["SettlementCurrencyID"] = PnrCurrencies.RUBCurrencyID;
            card.Sections["PnrContracts"].Fields["SettlementCurrencyName"] = PnrCurrencies.RUBCurrencyName;
            card.Sections["PnrContracts"].Fields["SettlementCurrencyCode"] = PnrCurrencies.RUBCurrencyCode;

            // Дата заключения
            card.Sections["PnrContracts"].Fields["ProjectDate"] = DateTime.Now;
            // Подразделение автора
            Guid authorID = (Guid)card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
            var userDepartment = await PnrUserDepartmentHelper.GetDepartmentByUserID(context.DbScope, authorID);
            if (userDepartment != null && userDepartment.ID != null)
            {
                card.Sections["PnrContracts"].Fields["DepartmentID"] = userDepartment.ID;
                card.Sections["PnrContracts"].Fields["DepartmentIdx"] = userDepartment.Idx;
                card.Sections["PnrContracts"].Fields["DepartmentName"] = userDepartment.Name;
                card.Sections["PnrContracts"].Fields["CFOID"] = userDepartment.CFOID;
                card.Sections["PnrContracts"].Fields["CFOName"] = userDepartment.CFOName;
            }

            // Ставка НДС 20%
            card.Sections["PnrContracts"].Fields["VATRateID"] = PnrVatRates.PnrVatRate20_ID;
            card.Sections["PnrContracts"].Fields["VATRateValue"] = PnrVatRates.PnrVatRate20_Value;
        }
    }
}
