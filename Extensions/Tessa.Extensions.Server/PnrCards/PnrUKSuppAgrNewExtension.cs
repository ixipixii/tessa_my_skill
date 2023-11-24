using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKSuppAgrNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            await TransferFieldsFromBasedOnCard(context, card);

            card.Sections["PnrSupplementaryAgreementsUK"].Fields["SettlementCurrencyID"] = PnrCurrencies.RUBCurrencyID;
            card.Sections["PnrSupplementaryAgreementsUK"].Fields["SettlementCurrencyName"] = PnrCurrencies.RUBCurrencyName;
            card.Sections["PnrSupplementaryAgreementsUK"].Fields["SettlementCurrencyCode"] = PnrCurrencies.RUBCurrencyCode;

            card.Sections["PnrSupplementaryAgreementsUK"].Fields["DevelopmentID"] = PnrDevelopment.DevelopmentNoID;
            card.Sections["PnrSupplementaryAgreementsUK"].Fields["DevelopmentName"] = PnrDevelopment.DevelopmentNoName;
        }

        /// <summary>
        /// Создание ДС УК на основании Договора УК. Перенос полей.
        /// </summary>
        private async Task TransferFieldsFromBasedOnCard(ICardNewExtensionContext context, Card card)
        {
            if (context.Request.Info.TryGetValue("KrCreateBasedOnCardID", out var basedOnCardID))
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;
                    var contractUkFields = await db.SetCommand(@"
						SELECT
						    pcuk.Subject,
						    pcuk.OrganizationID,
                            pcuk.OrganizationName,
						    pcuk.PartnerID,
						    pcuk.PartnerName,
						    pcuk.CFOID,
						    pcuk.CFOName,
						    dci.FullNumber
                        FROM PnrContractsUK pcuk
                        JOIN DocumentCommonInfo dci ON dci.id = pcuk.id
                        WHERE pcuk.[ID] = @CardID",
                            db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteAsync<PnrContractsFields>();

                    if (contractUkFields != null && card.Sections.ContainsKey("PnrSupplementaryAgreementsUK"))
                    {
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["Subject"] = contractUkFields.Subject;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["OrganizationID"] = contractUkFields.OrganizationID;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["OrganizationName"] = contractUkFields.OrganizationName;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["PartnerID"] = contractUkFields.PartnerID;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["PartnerName"] = contractUkFields.PartnerName;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["CFOID"] = contractUkFields.CFOID;
                        card.Sections["PnrSupplementaryAgreementsUK"].Fields["CFOName"] = contractUkFields.CFOName;
                        card.Sections["DocumentCommonInfo"].Fields["FullNumber"] = contractUkFields.FullNumber;
                    }
                }
            }
        }

        private class PnrContractsFields
        {
            public string Subject { get; set; }
            public Guid? OrganizationID { get; set; }
            public string OrganizationName { get; set; }
            public Guid? PartnerID { get; set; }
            public string PartnerName { get; set; }
            public Guid? CFOID { get; set; }
            public string CFOName { get; set; }
            public string FullNumber { get; set; }
        }
    }
}
