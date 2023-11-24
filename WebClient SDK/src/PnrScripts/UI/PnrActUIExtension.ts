import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrActTypes } from '../Shared/PnrActTypes';
import Moment from 'moment';

export class PnrActUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrActTypeID)) {
      return;
    }
    let pnrActs = context.card.sections.tryGet("PnrActs");
    if (!pnrActs) return;

    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
        const projectDateControl = context.model.controls.get('ProjectDate') as DateTimeViewModel;
        if (!projectDateControl) return;
        // Заполнить поле Дата проекта текущей датой
        projectDateControl.selectedDate = Moment(Date.now());
    }

    pnrActs.fields.fieldChanged.add(async (e) => {
      // Тип акта: обязательность заполнения полей
      if (e.fieldName == "TypeID" && e.fieldValue != null)
      {
         this.ChangeControlsActiveValidation(e.fieldValue, context);
      }
    });
  }

  /** валидация полей, зависимых от Типа акта */
  private ChangeControlsActiveValidation(typeID, context: ICardUIExtensionContext)
  {
    // Стадия реализации
    const implementationStage = context.model.controls.get('ImplementationStage');
    if (implementationStage) {
      implementationStage.isRequired = typeID == PnrActTypes.PnrAcceptanceCertificateTypeID;
      implementationStage.notifyUpdateValidation();
    }
  }
}