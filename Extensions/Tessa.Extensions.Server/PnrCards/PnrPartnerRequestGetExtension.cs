using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.Integration;

namespace Tessa.Extensions.Server.PnrCards
{
    class PnrPartnerRequestGetExtension : CardGetExtension
    {
        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;

            if (!context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }
        }
    }
}
