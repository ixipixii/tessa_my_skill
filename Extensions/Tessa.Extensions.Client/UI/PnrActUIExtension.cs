using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrActUIExtension : CardUIExtension
    {
        /// <summary>
        /// Валидация контролов.
        /// </summary>
        private void ChangeControlsActiveValidation(Guid typeID, ICardModel cardModel)
        {
            // Стадия реализации
            if (cardModel.Controls.TryGet("ImplementationStage", out IControlViewModel implementationStage))
            {
                implementationStage.IsRequired = typeID == PnrActTypes.PnrAcceptanceCertificateTypeID;
                implementationStage.NotifyUpdateValidation();
            }
        }
        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата проекта текущей датой
                if (cardModel.Controls.TryGet("ProjectDate", out IControlViewModel projectDateControl))
                {
                    ((DateTimeViewModel)projectDateControl).SelectedDate = DateTime.Now;
                }
            }

            context.Card.Sections["PnrActs"].FieldChanged += (s, e) =>
            {
                // Тип акта: обязательность заполнения полей
                if (e.FieldName == "TypeID" && e.FieldValue != null)
                {
                    ChangeControlsActiveValidation((Guid)e.FieldValue, cardModel);
                }
            };

            return base.Initialized(context);
        }
    }
}
