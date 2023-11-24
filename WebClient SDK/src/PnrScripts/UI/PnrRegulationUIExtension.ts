import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import { CardRequest, CardService } from 'tessa/cards/service';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import Moment from 'moment';

export class PnrRegulationUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrRegulationTypeID)) {
      return;
    }
    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (!regDateControl) return;
      // Заполнить поле Дата регистрации текущей датой
      regDateControl.selectedDate = Moment(Date.now());

      // ЦФО(по Инициатору) при создании карточки
      let dci = context.card.sections.tryGet("DocumentCommonInfo");
      if (dci != null) {
        await this.SetUserDepartmentInfo(dci.fields.get('AuthorID'), context);
      }
    }
  }

  private async SetUserDepartmentInfo(authorID, context: ICardUIExtensionContext) {
    let CFOID;
    let CFOName;
    let request = new CardRequest();
        request.requestType = PnrRequestTypes.GetUserDepartmentInfoRequestTypeID;
        request.info["authorID"] = createTypedField(authorID, DotNetType.Guid);
    // проверка ответа
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
      CFOID = response.info.CFOID;
      CFOName = response.info.CFOName;
    }

    let PnrRegulations = context.card.sections.tryGet("PnrRegulations");
    if (CFOID != null && CFOName != null && PnrRegulations != null) {
      PnrRegulations.fields.set('CFOID', createTypedField(CFOID.$value, DotNetType.Guid))
      PnrRegulations.fields.set('CFOName', createTypedField(CFOName.$value, DotNetType.String))
    }
  }
}