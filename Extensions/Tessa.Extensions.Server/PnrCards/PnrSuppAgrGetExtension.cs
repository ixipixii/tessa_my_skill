using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrSuppAgrGetExtension : CardGetExtension
    {
        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            decimal amountContract = 0;
            decimal amountSA = 0;

            if (card.Sections.TryGetValue("PnrSupplementaryAgreements", out CardSection pnrSupplementaryAgreements))
            {
                if (pnrSupplementaryAgreements.Fields.ContainsKey("MainContractID") && card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"] != null)
                {
                    var contractID = (Guid)card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"];

                    await using (context.DbScope.Create())
                    {
                        var db = context.DbScope.Db;
                        amountContract = await db.SetCommand(
                            "SELECT Amount FROM PnrContracts AS C "
                            + "WHERE C.ID = @contractID;",
                        db.Parameter("contractID", contractID))
                        .LogCommand()
                        .ExecuteAsync<decimal>();
                    }
                }

                if (pnrSupplementaryAgreements.Fields.ContainsKey("AmountSA") && card.Sections["PnrSupplementaryAgreements"].Fields["AmountSA"] != null)
                {
                    amountSA = (decimal)card.Sections["PnrSupplementaryAgreements"].Fields["AmountSA"];
                }

                card.Sections["PnrSupplementaryAgreements"].Fields["Amount"] = amountSA + amountContract;
                card.Sections["PnrSupplementaryAgreements"].SetChanged("Amount", false);
            }
        }
    }
}
