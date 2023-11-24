import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel, AutoCompleteTableViewModel } from 'tessa/ui/cards/controls';
import { PnrOutgoingTypes } from '../Shared/PnrOutgoingTypes';
import { PnrLegalEntityIndex } from '../Shared/PnrLegalEntityIndex';
import Moment from 'moment';

export class PnrOutgoingUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrOutgoingTypeID)) {
      return;
    }

    const OrganizationGKControl = context.model.controls.get("Organizations") as AutoCompleteTableViewModel;
    const PnrOutgoing = context.card.sections.tryGet('PnrOutgoing');
    const PnrOutgoingOrganizations = context.card.sections.tryGet('PnrOutgoingOrganizations');

    // Подпишемся на изменения коллекции организаций
    if (OrganizationGKControl){
      OrganizationGKControl.valueSet.add(() => {this.SetLegalEntityIndex(OrganizationGKControl, context); });
      OrganizationGKControl.valueDeleted.add(() => {this.SetLegalEntityIndex(OrganizationGKControl, context); });
    }

    // Создание карточки
    if (context.card.storeMode === CardStoreMode.Insert) {
      const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (!regDateControl) return;
      // Заполнить поле Дата регистрации текущей датой
      regDateControl.selectedDate = Moment(Date.now());

      let dci = context.card.sections.tryGet('DocumentCommonInfo');
      if (dci != null) {
        // начальная инициализация поля Подразделение по автору
        await this.SetUserDepartmentInfo(dci.fields.get('AuthorID'), context);
      }
      // поле Вид исходящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrOutgoing!.fields.get('DocumentKindIdx'), context);

      // Если карточка скопирована то индекс переносится из предыдущей, однако пользователи могли уже изменить скцию организаций ГК
      // чтобы не было чудес проверяем при инициализации карточки индекс и актуализируем его.
      if (!PnrOutgoingOrganizations || !PnrOutgoing) return;

      if (OrganizationGKControl.items.length > 1) {
        PnrOutgoing.fields.set(
          "LegalEntityIndexIdx",
          createTypedField(PnrLegalEntityIndex.WholeOrganizationIdx, DotNetType.String));
      }
      else if (OrganizationGKControl.items.length === 1) {
        this.SetLegalEntityIndex(OrganizationGKControl, context);
      }
      else{
        PnrOutgoing.fields.set(
          "LegalEntityIndexIdx",
          null);
      }
    } else if (PnrOutgoing!.fields.get('DocumentKindID') != null) {
      // Открытие карточки. Есть Вид исходящего документа - устанавливаем видимость контролов в зависимости от Вида
      this.SetDKDependenceVisibility(PnrOutgoing!.fields.get('DocumentKindIdx'), context);
    }

    PnrOutgoing!.fields.fieldChanged.add(async e => {
      // Вид исходящего документа: Visibility контролов и переформирование номера
      if (e.fieldName === 'DocumentKindIdx' && e.fieldValue != null) {
        this.SetDKDependenceVisibility(e.fieldValue, context);
      }
    });
  }

  private async SetLegalEntityIndex(OrganizationGKControl: AutoCompleteTableViewModel, context: ICardUIExtensionContext) {
    const PnrOutgoing = context.card.sections.tryGet('PnrOutgoing');
    if (!PnrOutgoing) return;

    if (OrganizationGKControl.items.length === 1) {
      let organizationID = OrganizationGKControl.items[0].columnValues[0];
      let request = new CardRequest();
      request.requestType = PnrRequestTypes.GetIndexLegalEntityRequestTypeID;
      request.info['organizationID'] = createTypedField(organizationID, DotNetType.Guid);
      let response = await CardService.instance.request(request);
      if (response.validationResult.isSuccessful) {
        let LegalEntityIdx = response.info.LegalEntityIdx;
        if (LegalEntityIdx) {
          PnrOutgoing.fields.set(
          "LegalEntityIndexIdx",
          createTypedField(LegalEntityIdx.$value, DotNetType.String)
          );
        }
        // Если в единственной указанной организации нет индекса то делаем поле LegalEntityIndexIdx пустым
        // чтобы при проверке на сервере в валидацию была возвращена ошибка индекса организации
        else {
        PnrOutgoing.fields.set(
          "LegalEntityIndexIdx",
          null);
        }
      }
    }
    else {
      PnrOutgoing.fields.set(
          "LegalEntityIndexIdx",
          createTypedField(PnrLegalEntityIndex.WholeOrganizationIdx, DotNetType.String)
        );
    }
  }

  private async SetUserDepartmentInfo(authorID, context: ICardUIExtensionContext) {
    let DepartmentID;
    let DepartmentName;
    let DepartmentIdx;
    let request = new CardRequest();
    request.requestType = PnrRequestTypes.GetUserDepartmentInfoRequestTypeID;
    request.info['authorID'] = createTypedField(authorID, DotNetType.Guid);

    // проверка ответа
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
      DepartmentID = response.info.DepartmentID;
      DepartmentName = response.info.Name;
      DepartmentIdx = response.info.Index;
    }

    let PnrOutgoing = context.card.sections.tryGet('PnrOutgoing');
    if (
      DepartmentID != null &&
      DepartmentName != null &&
      DepartmentIdx != null &&
      PnrOutgoing != null &&
      PnrOutgoing.fields.get('DepartmentID') == null
    ) {
      PnrOutgoing.fields.set(
        'DepartmentID',
        createTypedField(DepartmentID.$value, DotNetType.Guid)
      );
      PnrOutgoing.fields.set(
        'DepartmentName',
        createTypedField(DepartmentName.$value, DotNetType.String)
      );
      PnrOutgoing.fields.set(
        'DepartmentIdx',
        createTypedField(DepartmentIdx.$value, DotNetType.String)
      );
    }
  }

  /** установка видимости контролов, зависимых от Вида исходящего документа */
  private SetDKDependenceVisibility(documentKindIdx, context: ICardUIExtensionContext) {
    const destination = context.model.controls.get('Destination');
    if (destination) {
      destination.controlVisibility =
        documentKindIdx === PnrOutgoingTypes.OutgoingLetterIdx
          ? Visibility.Visible
          : Visibility.Collapsed;
    }
    const destinationFIO = context.model.controls.get('DestinationFIO');
    if (destinationFIO) {
      destinationFIO.controlVisibility =
        documentKindIdx === PnrOutgoingTypes.OutgoingLetterIdx
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    const signatory = context.model.controls.get('Signatory');
    if (signatory) {
      signatory.controlVisibility =
        documentKindIdx === PnrOutgoingTypes.OutgoingLetterIdx
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    const block = context.model.blocks.get('RepliesComplaintsBlock');
    if (!block) return;
    block.blockVisibility =
      documentKindIdx === PnrOutgoingTypes.OutgoingComplaintsIdx
        ? Visibility.Visible
        : Visibility.Collapsed;

    const OutgoingLetterBlock = context.model.blocks.get('OutgoingLetterBlock');
    if (!OutgoingLetterBlock) return;
    OutgoingLetterBlock.blockVisibility =
      documentKindIdx === PnrOutgoingTypes.OutgoingLetterIdx
        ? Visibility.Visible
        : Visibility.Collapsed;

    // валидация Доп.согласующие - только в Исходящее письмо
    const performers = context.model.controls.get('Performers');
    if (performers) {
      //performers.hasActiveValidation = false;
      performers.isRequired = documentKindIdx === PnrOutgoingTypes.OutgoingLetterIdx;
      performers.notifyUpdateValidation();
    }
  }
}
