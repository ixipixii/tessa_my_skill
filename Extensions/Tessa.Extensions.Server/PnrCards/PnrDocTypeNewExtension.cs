using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    // установка Тип документа при создании карточки
    public sealed class PnrDocTypeNewExtension : CardNewExtension
    {
        public override Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.Card;

            card.Sections["DocumentCommonInfo"].Fields["CardTypeID"] = card.TypeID;
            card.Sections["DocumentCommonInfo"].Fields["CardTypeCaption"] = card.TypeCaption;

            return Task.CompletedTask;
        }
    }
}
