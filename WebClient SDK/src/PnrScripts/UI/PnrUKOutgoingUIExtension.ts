import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrOutgoingUKTypes } from '../Shared/PnrOutgoingUKTypes';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';

export class PnrUKOutgoingUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrOutgoingUKTypeID)) {
      return;
    }
    let PnrOutgoingUK = context.card.sections.tryGet("PnrOutgoingUK");
    if (!PnrOutgoingUK) return;
    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (!regDateControl) return;
      // Заполнить поле Дата регистрации текущей датой
      regDateControl.selectedDate = Moment(Date.now());

      let dci = context.card.sections.tryGet("DocumentCommonInfo");
      if (dci != null) {
        // начальная инициализация поля Подразделение по автору
        await this.SetUserDepartmentInfo(dci.fields.get('AuthorID'), context);
      }
      // поле Вид исходящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrOutgoingUK.fields.get('DocumentKindIdx'), context);
    }
    else if (PnrOutgoingUK.fields.get('DocumentKindID') != null) {
      // Открытие карточки. Есть Вид исходящего документа - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrOutgoingUK.fields.get('DocumentKindIdx'), context);
    }

    PnrOutgoingUK.fields.fieldChanged.add(async (e) => {
      // Вид исходящего документа: Visibility контролов и переформирование номера
      if (e.fieldName == "DocumentKindIdx" && e.fieldValue != null)
      {
         this.SetDKDependenceVisibility(e.fieldValue, context);
      }
      // Организация ГК Пионер: заполнение Индекс ЮЛ + изменение Номера
      if (e.fieldName == "OrganizationID" && e.fieldValue != null)
      {
        await this.SetLegalEntityIndex(e.fieldValue, context);
      }
    });
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
    let PnrOutgoingUK = context.card.sections.tryGet("PnrOutgoingUK");
    if (LegalEntityIdx != null && PnrOutgoingUK != null) {
      PnrOutgoingUK.fields.set('LegalEntityIndexIdx', createTypedField(LegalEntityIdx.$value, DotNetType.String))
    }
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

    let PnrOutgoingUK = context.card.sections.tryGet("PnrOutgoingUK");
    if (DepartmentID != null && DepartmentName != null && DepartmentIdx != null && PnrOutgoingUK != null && PnrOutgoingUK.fields.get('DepartmentID') == null) {
      PnrOutgoingUK.fields.set('DepartmentID', createTypedField(DepartmentID.$value, DotNetType.Guid));
      PnrOutgoingUK.fields.set('DepartmentName', createTypedField(DepartmentName.$value, DotNetType.String));
      PnrOutgoingUK.fields.set('DepartmentIdx', createTypedField(DepartmentIdx.$value, DotNetType.String));
    }
  }

  /** Установка видимости контролов, зависимых от Вида исходящего документа УК ПС */
  private SetDKDependenceVisibility(documentKindIdx, context: ICardUIExtensionContext)
  {
    const incomingDocsBlock = context.model.blocks.get('OutgoingDocsBlock');
    if (incomingDocsBlock) {
      incomingDocsBlock.blockVisibility = documentKindIdx == PnrOutgoingUKTypes.OutgoingUKLetterIdx ? Visibility.Visible : Visibility.Collapsed;
    }
    const complaintsBlock = context.model.blocks.get('ComplaintsBlock');
    if (complaintsBlock) {
      complaintsBlock.blockVisibility = documentKindIdx == PnrOutgoingUKTypes.OutgoingUKComplaintsIdx ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}