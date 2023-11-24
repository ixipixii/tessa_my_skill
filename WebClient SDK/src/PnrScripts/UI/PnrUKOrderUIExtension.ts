import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';

export class PnrUKOrderUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrOrderUKTypeID)) {
      return;
    }
    let PnrOrderUK = context.card.sections.tryGet("PnrOrderUK");
    if (!PnrOrderUK) return;
    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
      // Заполнить поле Дата регистрации текущей датой
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (regDateControl) {
        regDateControl.selectedDate = Moment(Date.now());
      }
      let dci = context.card.sections.tryGet("DocumentCommonInfo");
      if (dci != null) {
        // начальная инициализация поля Подразделение по автору
        await this.SetUserDepartmentInfo(dci.fields.get('AuthorID'), context);
      }
    }

    PnrOrderUK.fields.fieldChanged.add(async (e) => {
      // Организация ГК Пионер: заполнение Индекс ЮЛ + изменение Номера
      if (e.fieldName == "OrganizationID" && e.fieldValue != null)
      {
        await this.SetLegalEntityIndex(e.fieldValue, context);
      }
    });
  }

  private async SetUserDepartmentInfo(authorID, context: ICardUIExtensionContext)
  {
    let DepartmentID;
    let DepartmentName;
    let DepartmentIdx;
    let request = new CardRequest();
        request.requestType = PnrRequestTypes.GetUserDepartmentInfoRequestTypeID;
        request.info["authorID"] = createTypedField(authorID, DotNetType.Guid);

    // проверка ответа
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
      DepartmentID = response.info.DepartmentID;
      DepartmentName = response.info.Name;
      DepartmentIdx = response.info.Index;
    }

    let PnrOrderUK = context.card.sections.tryGet("PnrOrderUK");
    if (DepartmentID != null && DepartmentName != null && DepartmentIdx != null && PnrOrderUK != null) {
      PnrOrderUK.fields.set('DepartmentID', createTypedField(DepartmentID.$value, DotNetType.Guid));
      PnrOrderUK.fields.set('DepartmentName', createTypedField(DepartmentName.$value, DotNetType.String));
      PnrOrderUK.fields.set('DepartmentIdx', createTypedField(DepartmentIdx.$value, DotNetType.String));
    }
  }

  private async SetLegalEntityIndex(organizationID, context: ICardUIExtensionContext)
  {
    // request к организации получить индекс ЮЛ
    let LegalEntityIdx;
    let request = new CardRequest();
        request.requestType = PnrRequestTypes.GetIndexLegalEntityRequestTypeID;
        request.info["organizationID"] = createTypedField(organizationID, DotNetType.Guid);
    // проверка ответа
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
        LegalEntityIdx = response.info.LegalEntityIdx;
    }
    let PnrOrderUK = context.card.sections.tryGet("PnrOrderUK");
    if (LegalEntityIdx != null && PnrOrderUK != null) {
      PnrOrderUK.fields.set('LegalEntityIndexIdx', createTypedField(LegalEntityIdx.$value, DotNetType.String))
    }
  }
}