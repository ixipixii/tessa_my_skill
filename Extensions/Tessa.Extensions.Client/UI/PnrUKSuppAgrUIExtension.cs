using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrUKSuppAgrUIExtension : CardUIExtension
    {
        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel projectDateControl))
                {
                    ((DateTimeViewModel)projectDateControl).SelectedDate = DateTime.Now;
                }
            }

            return base.Initialized(context);
        }
    }
}
