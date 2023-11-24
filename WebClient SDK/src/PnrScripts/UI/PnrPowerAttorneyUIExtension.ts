import { CardUIExtension, ICardUIExtensionContext, IControlViewModel } from 'tessa/ui/cards';
import { Guid, Visibility } from 'tessa/platform';
import { PnrCardTypes } from '../Shared/PnrCardTypes';

export class PnrPowerAttorneyUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrPowerAttorneyTypeID)) {
      return;
    }
    let PnrPowerAttorney = context.card.sections.tryGet("PnrPowerAttorney");
    let confidants = context.model.controls.get('Confidants'); // Доверенное лицо
    let confidantNotEmployee = context.model.controls.get('ConfidantNotEmployee'); // Доверенное лицо (не сотрудник компании)
    if (!PnrPowerAttorney) return;
    
    if(confidants != null && confidantNotEmployee != null)
    {
      // добавляем валидацию контролы 'Доверенное лицо' и 'Доверенное лицо (не сотрудник компании)'
      //confidants.validationFunc = control => control != null && control.hasEmptyValue ? "Поле 'Доверенное лицо' обязательно для заполнения!" : null;
      //confidantNotEmployee.validationFunc = control => control != null && control.hasEmptyValue ? "Доверенное лицо (не сотрудник компании)' обязательно для заполнения!" : null;
      // видимость контролов при открытии карточки на редактирование
      if (PnrPowerAttorney.fields.get('EmployeeID') != null) {
        this.SetConfidantsVisibility(PnrPowerAttorney.fields.get('EmployeeID'), confidants, confidantNotEmployee);
      }
      // видимость контролов при смене значения "Сотрудник ГК Пионер"
      PnrPowerAttorney.fields.fieldChanged.add((e) => {
        if (e.fieldName == "EmployeeID" && e.fieldValue != null && confidants && confidantNotEmployee)
        {
          this.SetConfidantsVisibility(e.fieldValue, confidants, confidantNotEmployee);
        }
      });
    }
  }

  /** установка видимости контролов Доверенное лицо. 
   * employee: 1 - сотрудник, 0 - нет
  */
  private SetConfidantsVisibility(employeeID, confidants: IControlViewModel, confidantNotEmployee: IControlViewModel)
  {
    confidants.controlVisibility = employeeID == 1 ? Visibility.Visible : Visibility.Collapsed;
    confidantNotEmployee.controlVisibility = employeeID == 0 ? Visibility.Visible : Visibility.Collapsed;
  }
}