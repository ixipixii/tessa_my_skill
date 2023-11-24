using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.UI.Cards;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrErrandUIExtension : CardUIExtension
    {
        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Контролер значением из поля Автор поручения
                cardModel.Card.Sections["PnrErrands"].Fields["ControllerID"] = cardModel.Card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
                cardModel.Card.Sections["PnrErrands"].Fields["ControllerName"] = cardModel.Card.Sections["DocumentCommonInfo"].Fields["AuthorName"];
            }

            return base.Initialized(context);
        }
    }
}
