using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrPowerAttorneyUIExtension : CardUIExtension
    {
        // установка видимости контролов Доверенное лицо
        // employee: 1 - сотрудник, 0 - нет
        private void SetConfidantsVisibility(int employeeID, IControlViewModel confidants, IControlViewModel confidantNotEmployee)
        {
            confidants.ControlVisibility = employeeID == 1 ? Visibility.Visible : Visibility.Collapsed;
            confidantNotEmployee.ControlVisibility = employeeID == 0 ? Visibility.Visible : Visibility.Collapsed;
            confidants.Block.Rearrange();
        }
        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            IControlViewModel confidants;                // Доверенное лицо
            IControlViewModel confidantNotEmployee;     // Доверенное лицо (не сотрудник компании)
            cardModel.Controls.TryGet("Confidants", out confidants);
            cardModel.Controls.TryGet("ConfidantNotEmployee", out confidantNotEmployee);
            
            if(confidants != null && confidantNotEmployee != null)
            {
                // добавляем валидацию контролы 'Доверенное лицо' и 'Доверенное лицо (не сотрудник компании)'
                //confidants.ValidationFunc = c =>
                //    (((ControlViewModelBase)c).HasEmptyValue())
                //        ? $"Поле 'Доверенное лицо' обязательно для заполнения!"
                //        : null;
                //confidantNotEmployee.ValidationFunc = c =>
                //    (((ControlViewModelBase)c).HasEmptyValue())
                //        ? $"'Доверенное лицо (не сотрудник компании)' обязательно для заполнения!"
                //        : null;

                // видимость контролов при открытии карточки на редактирование
                if (card.Sections["PnrPowerAttorney"].Fields["EmployeeID"] != null)
                {
                    int employeeID = (int)card.Sections["PnrPowerAttorney"].Fields["EmployeeID"];
                    SetConfidantsVisibility(employeeID, confidants, confidantNotEmployee);
                }

                // видимость контролов при смене значения "Сотрудник ГК Пионер"
                context.Card.Sections["PnrPowerAttorney"].FieldChanged += (s, e) =>
                {
                    if (e.FieldName == "EmployeeID" && e.FieldValue != null)
                    {
                        SetConfidantsVisibility((int)e.FieldValue, confidants, confidantNotEmployee);
                    }
                };
            }

            return base.Initialized(context);
        }
    }
}
