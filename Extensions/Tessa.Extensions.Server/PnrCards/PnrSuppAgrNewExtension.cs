using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrSuppAgrNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            // KrCreateBasedOnCardID
            Card card = context.Response.Card;

            await TransferFieldsFromBasedOnCard(context, card);
            // Валюта (RUB)
            card.Sections["PnrSupplementaryAgreements"].Fields["SettlementCurrencyID"] = PnrCurrencies.RUBCurrencyID;
            card.Sections["PnrSupplementaryAgreements"].Fields["SettlementCurrencyName"] = PnrCurrencies.RUBCurrencyName;
            card.Sections["PnrSupplementaryAgreements"].Fields["SettlementCurrencyCode"] = PnrCurrencies.RUBCurrencyCode;

            object kindID, kindName, kindDUPID, kindDUPName;
            context.Request.Info.TryGetValue("KindID", out kindID);
            context.Request.Info.TryGetValue("KindName", out kindName);
            context.Request.Info.TryGetValue("KindDUPID", out kindDUPID);
            context.Request.Info.TryGetValue("KindDUPName", out kindDUPName);

            if (kindID != null)
            {
                card.Sections["PnrSupplementaryAgreements"].Fields["KindID"] = (Guid)kindID;
                card.Sections["PnrSupplementaryAgreements"].Fields["KindName"] = (string)kindName;
                card.Permissions.Sections.GetOrAdd("PnrSupplementaryAgreements").FieldPermissions["KindID"] = CardPermissionFlags.ProhibitModify;
            }

            if (kindDUPID != null)
            {
                card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPID"] = (Guid)kindDUPID;
                card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPName"] = (string)kindDUPName;
                card.Permissions.Sections.GetOrAdd("PnrSupplementaryAgreements").FieldPermissions["KindDUPID"] = CardPermissionFlags.ProhibitModify;
            }

            // Дата заключения
            card.Sections["PnrSupplementaryAgreements"].Fields["ProjectDate"] = DateTime.Now;
            // Подразделение автора
            Guid authorID = (Guid)card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
            var userDepartment = await PnrUserDepartmentHelper.GetDepartmentByUserID(context.DbScope, authorID);
            if (userDepartment != null && userDepartment.ID != null)
            {
                card.Sections["PnrSupplementaryAgreements"].Fields["DepartmentID"] = userDepartment.ID;
                card.Sections["PnrSupplementaryAgreements"].Fields["DepartmentIdx"] = userDepartment.Idx;
                card.Sections["PnrSupplementaryAgreements"].Fields["DepartmentName"] = userDepartment.Name;
            }

            // Ставка НДС 20%
            card.Sections["PnrSupplementaryAgreements"].Fields["VATRateID"] = PnrVatRates.PnrVatRate20_ID;
            card.Sections["PnrSupplementaryAgreements"].Fields["VATRateValue"] = PnrVatRates.PnrVatRate20_Value;
        }

        /// <summary>
        /// Создание ДС на основании Договора. Перенос полей.
        /// </summary>
        private async Task TransferFieldsFromBasedOnCard(ICardNewExtensionContext context, Card card)
        {
            if (context.Request.Info.TryGetValue("KrCreateBasedOnCardID", out var basedOnCardID))
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;
                    var contractFields = await db.SetCommand(@"
                        SELECT
                            pc.Subject, 
                            pc.PartnerID,
                            pc.PartnerName,
                            pc.OrganizationID,
                            pc.OrganizationName,
                            pc.CFOID,
                            pc.CFOName,
                            pc.ProjectID,
                            pc.ProjectName,
                            pc.CostItemID,
                            pc.CostItemName,
                            pc.KindID,
                            pc.KindName,
                            pc.KindDUPID,
                            pc.KindDUPName,
                            pc.ID as PnrContractID,
                            CONCAT(dci.CardTypeCaption, ': ',
                            CONCAT(dci.FullNumber, ' от ', 
                            pc.ProjectDate)) as PnrContractName,
						    pc.SignatoryID,
						    pc.SignatoryName
                        FROM [PnrContracts] pc WITH(NOLOCK)
                        JOIN DocumentCommonInfo dci WITH(NOLOCK)
                            on dci.id = pc.id
                        WHERE pc.[ID] = @CardID",
                            db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteAsync<PnrContractsFields>();

                    if (contractFields != null && card.Sections.ContainsKey("PnrSupplementaryAgreements"))
                    {
                        card.Sections["PnrSupplementaryAgreements"].Fields["Subject"] = contractFields.Subject;
                        card.Sections["PnrSupplementaryAgreements"].Fields["PartnerID"] = contractFields.PartnerID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["PartnerName"] = contractFields.PartnerName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["OrganizationID"] = contractFields.OrganizationID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["OrganizationName"] = contractFields.OrganizationName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["PartnerName"] = contractFields.PartnerName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["CFOID"] = contractFields.CFOID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["CFOName"] = contractFields.CFOName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["ProjectID"] = contractFields.ProjectID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["ProjectName"] = contractFields.ProjectName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["CostItemID"] = contractFields.CostItemID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["CostItemName"] = contractFields.CostItemName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["KindID"] = contractFields.KindID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["KindName"] = contractFields.KindName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPID"] = contractFields.KindDUPID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["KindDUPName"] = contractFields.KindDUPName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"] = basedOnCardID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["MainContractSubject"] = contractFields.PnrContractName;
                        card.Sections["PnrSupplementaryAgreements"].Fields["SignatoryID"] = contractFields.SignatoryID;
                        card.Sections["PnrSupplementaryAgreements"].Fields["SignatoryName"] = contractFields.SignatoryName;
                    }
                }
            }
        }

        private class PnrContractsFields
        {
            public string Subject { get; set; }
            public Guid? PartnerID { get; set; }
            public string PartnerName { get; set; }
            public Guid? OrganizationID { get; set; }
            public string OrganizationName { get; set; }
            public Guid? CFOID { get; set; }
            public string CFOName { get; set; }
            public Guid? ProjectID { get; set; }
            public string ProjectName { get; set; }
            public Guid? CostItemID { get; set; }
            public string CostItemName { get; set; }
            public Guid? KindID { get; set; }
            public string KindName { get; set; }
            public Guid? KindDUPID { get; set; }
            public string KindDUPName { get; set; }
            public Guid? PnrContractID { get; set; }
            public string PnrContractName { get; set; }
            public Guid? SignatoryID { get; set; }
            public string SignatoryName { get; set; }
        }
    }
}
