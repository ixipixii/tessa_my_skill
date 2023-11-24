import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';

export class PnrUKIncomingUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrIncomingUKTypeID)) {
      return;
    }
    let PnrIncomingUK = context.card.sections.tryGet("PnrIncomingUK");
    if (!PnrIncomingUK) return;
    // Создание карточки
    if (context.card.storeMode == CardStoreMode.Insert)
    {
      // Заполнить поле Дата регистрации текущей датой(можно просто продублировать значение из Дата создания)
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (regDateControl) {
        regDateControl.selectedDate = Moment(Date.now());
      }
      // поле Вид входящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrIncomingUK.fields.get('DocumentKindIdx'), context);
    }
    else if (PnrIncomingUK.fields.get('DocumentKindID') != null)
    {
      // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrIncomingUK.fields.get('DocumentKindID'), context);
    }

    PnrIncomingUK.fields.fieldChanged.add(async (e) => {
       // Вид входящего документа: Visibility контролов
       if (e.fieldName == "DocumentKindIdx" && e.fieldValue != null)
       {
          this.SetDKDependenceVisibility(e.fieldValue, context);
       }
       // Организация ГК Пионер: заполнение Индекс ЮЛ
       if (e.fieldName == "OrganizationID" && e.fieldValue != null)
       {
          this.SetLegalEntityIndex(e.fieldValue, context);
       }
    });
  }

  /** Установка видимости контролов, зависимых от Вида входящего документа УК ПС */
  private SetDKDependenceVisibility(documentKindIdx, context: ICardUIExtensionContext)
  {
    const incomingDocsBlock = context.model.blocks.get('IncomingDocsBlock');
    if (incomingDocsBlock) {
      incomingDocsBlock.blockVisibility = documentKindIdx == "0-01" ? Visibility.Visible : Visibility.Collapsed;
    }
    const complaintsBlock = context.model.blocks.get('ComplaintsBlock');
    if (complaintsBlock) {
      complaintsBlock.blockVisibility = documentKindIdx == "0-02" ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  /** Установка значения скрытого поля Индекс ЮЛ */
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
    let PnrIncomingUK = context.card.sections.tryGet("PnrIncomingUK");
    if (LegalEntityIdx != null && PnrIncomingUK != null) {
      PnrIncomingUK.fields.set('LegalEntityIndexIdx', createTypedField(LegalEntityIdx.$value, DotNetType.String))
    }
  }
}