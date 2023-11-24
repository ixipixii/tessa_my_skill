using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKIncomingNewExtension : CardNewExtension
    {
        public override Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.Card;

            card.Sections["PnrIncomingUK"].Fields["DocumentKindID"] = PnrIncomingUKTypes.IncomingUKLetterID;
            card.Sections["PnrIncomingUK"].Fields["DocumentKindIdx"] = PnrIncomingUKTypes.IncomingUKLetterIdx;
            card.Sections["PnrIncomingUK"].Fields["DocumentKindName"] = PnrIncomingUKTypes.IncomingUKLetterName;

            return Task.CompletedTask;
        }
    }
}
