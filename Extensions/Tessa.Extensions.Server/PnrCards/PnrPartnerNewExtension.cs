using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrPartnerNewExtension : CardNewExtension
    {
        public override Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.Card;

            card.Sections["Partners"].Fields["DateCreation"] = DateTime.Now;
            // card.Sections["Partners"].Fields["Validity"] = DateTime.Now.AddMonths(6);

            return Task.CompletedTask;
        }
    }
}