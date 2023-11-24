using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKContractNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            card.Sections["PnrContractsUK"].Fields["SettlementCurrencyID"] = PnrCurrencies.RUBCurrencyID;
            card.Sections["PnrContractsUK"].Fields["SettlementCurrencyName"] = PnrCurrencies.RUBCurrencyName;
            card.Sections["PnrContractsUK"].Fields["SettlementCurrencyCode"] = PnrCurrencies.RUBCurrencyCode;

            card.Sections["PnrContractsUK"].Fields["DevelopmentID"] = PnrDevelopment.DevelopmentNoID;
            card.Sections["PnrContractsUK"].Fields["DevelopmentName"] = PnrDevelopment.DevelopmentNoName;
        }
    }
}
