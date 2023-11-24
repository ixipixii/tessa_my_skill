using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrTemplateNewExtension : CardNewExtension
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
                card.Sections["PnrTemplates"].Fields["ProjectDate"] = DateTime.Now;

                // Шаблон: значение по умолчанию "К публикации в CRM"
                // Справочник PnrTemplateTypes - типа Перечисление. При изменении на Карточки - изменить установку значения по умолчанию
                card.Sections["PnrTemplates"].Fields["TemplateID"] = 0;
                card.Sections["PnrTemplates"].Fields["TemplateName"] = "К публикации CRM";

                // Ответственный - Автор
                card.Sections["PnrTemplates"].Fields["ResponsibleID"] = card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
                card.Sections["PnrTemplates"].Fields["ResponsibleName"] = card.Sections["DocumentCommonInfo"].Fields["AuthorName"];
            }

            return Task.CompletedTask;
        }
    }
}
