import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { Card, CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrIncomingTypes } from '../Shared/PnrIncomingTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrLegalEntityIndex } from '../Shared/PnrLegalEntityIndex';
import { CardRequest, CardService } from 'tessa/cards/service';
import { AutoCompleteTableViewModel, RowAutoCompleteItem, DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';
import { userSession }  from 'common/utility';
import { PnrMetadataContainerClerk, PnrMetadataContainerOfficeManager } from 'src/PnrScripts/UI/PnrMetadataExtension';

export class PnrIncomingUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrIncomingTypeID)) {
      return;
    }
    let PnrIncoming = context.card.sections.tryGet("PnrIncoming");
    if (!PnrIncoming) return;

    const isCurrentUserAdmin = userSession.isAdmin;
    const isCurrentUserClerk = PnrMetadataContainerClerk.instance.isUserInRole;
    const isCurrentUserOfficeManager = PnrMetadataContainerOfficeManager.instance.isUserInRole;

    const documentKind = context.model.controls.get('DocumentKind');
    if (documentKind) {
      if (isCurrentUserAdmin || (isCurrentUserClerk && isCurrentUserOfficeManager)) {
        documentKind.isReadOnly = false;
      }
      else {
        documentKind.isReadOnly = true;
      }
    }

    // Создание карточки
    if (context.card.storeMode === CardStoreMode.Insert)
    {
      // Заполнить поле Дата регистрации текущей датой(можно просто продублировать значение из Дата создания)
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (regDateControl) {
        regDateControl.selectedDate = Moment(Date.now());
      }
      // поле Вид входящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrIncoming.fields.get('DocumentKindIdx'), context);
    }
    else if (PnrIncoming.fields.get('DocumentKindID') != null)
    {
      // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrIncoming.fields.get('DocumentKindIdx'), context);
    }
    const organizations = context.model.controls.get('Organizations') as AutoCompleteTableViewModel;
    // Организация ГК Пионер
    if (organizations) {
      organizations.valueSet.add(async () => {
        await this.SetLegalEntityIndex(organizations, context);
      });
      organizations.valueDeleted.add(async () => {
        await this.SetLegalEntityIndex(organizations, context);
      });
    }

    if (PnrIncoming.fields.get('DocumentKindID') === PnrIncomingTypes.IncomingComplaintsID &&
     PnrIncoming.fields.get('DepartmentID') === null) {
      this.SetAutorDepartment(PnrIncomingTypes.IncomingComplaintsID, context.card);
    }

    PnrIncoming.fields.fieldChanged.add(async (e) => {
       // Вид входящего документа: Visibility контролов
       if (e.fieldName === "DocumentKindIdx" && e.fieldValue != null) {
          this.SetDKDependenceVisibility(e.fieldValue, context);
       }
       if (e.fieldName === "DocumentKindID" && e.fieldValue != null) {
          this.SetAutorDepartment(e.fieldValue, context.card);
       }
    });
  }

  private async SetAutorDepartment(fieldValue, card: Card) {
    let PnrIncoming = card.sections.tryGet("PnrIncoming");
        if (!PnrIncoming) return;
    if (fieldValue === PnrIncomingTypes.IncomingComplaintsID) {
        let request = new CardRequest();
        request.requestType = PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
        request.info["authorID"] = createTypedField(card.createdById, DotNetType.Guid);
        let response = await CardService.instance.request(request);
        if (response.validationResult.isSuccessful) {
          PnrIncoming.fields.set('DepartmentID', createTypedField(response.info.DepartmentID.$value, DotNetType.Guid));
          PnrIncoming.fields.set('DepartmentName', createTypedField(response.info.Name.$value, DotNetType.String));
          PnrIncoming.fields.set('DepartmentIdx', createTypedField(response.info.Index.$value, DotNetType.String));
        }
      }
    else {
        PnrIncoming.fields.set('DepartmentID', null);
        PnrIncoming.fields.set('DepartmentName', null);
        PnrIncoming.fields.set('DepartmentIdx', null);
    }
  }

  /** Установка видимости контролов, зависимых от Вида входящего документа */
  private SetDKDependenceVisibility(documentKindIdx, context: ICardUIExtensionContext)
  {
    const correspondent = context.model.controls.get('Correspondent');
    if (correspondent) {
        correspondent.controlVisibility = documentKindIdx === "1-01" ? Visibility.Visible : Visibility.Collapsed;
    }

    const block = context.model.blocks.get('ComplaintsBlock');
    if (block) {
      block.blockVisibility = documentKindIdx === "1-02" ? Visibility.Visible : Visibility.Collapsed;
    }

    const IncomingLetterBlock = context.model.blocks.get('IncomingLetterBlock');
    if (!IncomingLetterBlock) return;
    IncomingLetterBlock.blockVisibility = documentKindIdx === "1-01" ? Visibility.Visible : Visibility.Collapsed;
    }

  /** Установка значения скрытого поля Индекс ЮЛ */
  private async SetLegalEntityIndex(organizationControl: AutoCompleteTableViewModel, context: ICardUIExtensionContext)
  {
    if (organizationControl.items.length === 1) {
      let rowAutoCompleteItem = organizationControl.items[0] as RowAutoCompleteItem;
      let row = rowAutoCompleteItem.row;
      let organizationID = row.getField('OrganizationID');
      if (organizationID != null) {
        organizationID = organizationID.$value;
      }
      let LegalEntityIdx;
      let request = new CardRequest();
          request.requestType = PnrRequestTypes.GetIndexLegalEntityRequestTypeID;
          request.info["organizationID"] = createTypedField(organizationID, DotNetType.Guid);

      // проверка ответа
      let response = await CardService.instance.request(request);
      if (response.validationResult.isSuccessful) {
          LegalEntityIdx = response.info.LegalEntityIdx;
      }
      let PnrIncoming = context.card.sections.tryGet("PnrIncoming");
      if (LegalEntityIdx != null && PnrIncoming != null) {
        PnrIncoming.fields.set('LegalEntityIndexIdx', createTypedField(LegalEntityIdx.$value, DotNetType.String))
      }
    }
    else {
      let PnrIncoming = context.card.sections.tryGet("PnrIncoming");
      if (PnrIncoming != null) {
        PnrIncoming.fields.set('LegalEntityIndexIdx', createTypedField(PnrLegalEntityIndex.WholeOrganizationIdx, DotNetType.String))
      }
    }
  }
}