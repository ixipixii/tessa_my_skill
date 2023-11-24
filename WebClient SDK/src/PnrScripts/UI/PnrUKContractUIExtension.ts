import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import Moment from 'moment';

export class PnrUKContractUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrContractUKTypeID)) {
      return;
    }

    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
        const projectDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
        if (!projectDateControl) return;
        // Заполнить поле Дата регистрации текущей датой
        projectDateControl.selectedDate = Moment(Date.now());
    }
  }
}