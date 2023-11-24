import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrDocumentKinds } from '../Shared/PnrDocumentKinds';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';

export class PnrOrderUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrOrderTypeID)) {
      return;
    }
    let PnrOrder = context.card.sections.tryGet("PnrOrder");
    if (!PnrOrder) return;
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
    else { // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
      let documentKindID = PnrOrder.fields.get('DocumentKindID');
      if (documentKindID != null) {
        this.SetDKDependenceVisibility(documentKindID, context);
      }
    }

    PnrOrder.fields.fieldChanged.add(async (e) => {
      // Вид документа: Visibility контролов и переформирование номера
      if (e.fieldName == "DocumentKindID" && e.fieldValue != null)
      {
         this.SetDKDependenceVisibility(e.fieldValue, context);
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

    let PnrOrder = context.card.sections.tryGet("PnrOrder");
    if (DepartmentID != null && DepartmentName != null && DepartmentIdx != null && PnrOrder != null) {
      PnrOrder.fields.set('DepartmentID', createTypedField(DepartmentID.$value, DotNetType.Guid));
      PnrOrder.fields.set('DepartmentName', createTypedField(DepartmentName.$value, DotNetType.String));
      PnrOrder.fields.set('DepartmentIdx', createTypedField(DepartmentIdx.$value, DotNetType.String));
    }
  }

  /** установка видимости контролов, зависимых от Вида документа */
  private SetDKDependenceVisibility(documentKindID, context: ICardUIExtensionContext)
  {
    const disposalBlock = context.model.blocks.get('DisposalBlock');
    if (!disposalBlock) return;
    if (Guid.equals(documentKindID, PnrDocumentKinds.OrderAdministrativeActivity) ||
        Guid.equals(documentKindID, PnrDocumentKinds.OrderMainActivity) ||
        Guid.equals(documentKindID, PnrDocumentKinds.OrderMobileCommunications) ||
        Guid.equals(documentKindID, PnrDocumentKinds.OrderImplementation))
    {
      disposalBlock.blockVisibility = Visibility.Collapsed;
    }
    else if (Guid.equals(documentKindID, PnrDocumentKinds.Disposal))
    {
      disposalBlock.blockVisibility = Visibility.Visible;
    }
  }
}