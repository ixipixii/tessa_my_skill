using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    // установка в Исполнители автора карточки при создании документа
    public sealed class PnrSetPerformersNewExtension : CardNewExtension
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
                CardRow row = card.Sections["Performers"].Rows.Add();
                row.State = CardRowState.Inserted;
                row.RowID = Guid.NewGuid();
                row.Fields["UserID"] = card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
                row.Fields["UserName"] = card.Sections["DocumentCommonInfo"].Fields["AuthorName"];
            }

            return Task.CompletedTask;
        }
    }
}
