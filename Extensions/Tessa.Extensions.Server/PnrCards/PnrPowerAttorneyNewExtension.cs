using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrPowerAttorneyNewExtension : CardNewExtension
    {
        public override Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.Card;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата проекта текущей датой
                card.Sections["PnrPowerAttorney"].Fields["ProjectDate"] = DateTime.Now;
            }

            return Task.CompletedTask;
        }
    }
}
