import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode, Card } from 'tessa/cards';
import { CardRequest, CardService } from 'tessa/cards/service';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
//import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import { PnrContractKinds } from '../Shared/PnrContractKinds';
//import Moment from 'moment';

export class PnrSuppAgrUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrSupplementaryAgreementTypeID)) {
      return;
    }
    let PnrSupplementaryAgreements = context.card.sections.tryGet("PnrSupplementaryAgreements");
    if (!PnrSupplementaryAgreements) return;

    // Есть Вид договора - устанавливаем видимость контролов в зависимости от Вида
    let kindID = PnrSupplementaryAgreements.fields.get('KindID');
    if (kindID != null) {
      this.SetDKDependenceVisibility(kindID, context);
      this.ChangeControlsCaption(kindID, context);
      this.ChangeControlsActiveValidation(kindID, context);
    }
    if (PnrSupplementaryAgreements.fields.get('KindDUPID') != null){
      this.SetDK_DUPDependenceVisibility(PnrSupplementaryAgreements.fields.get('KindDUPID'), context);
    }

    if (context.card.storeMode == CardStoreMode.Update)
    { // Открытие карточки
      // Есть Изменение суммы договора - устанавливаем видимость Сумма договора с учетом ДС
      let isAmountChanged = PnrSupplementaryAgreements.fields.get('IsAmountChanged');
      if (isAmountChanged != null && isAmountChanged == true) {
        this.SetAmountVisibility(isAmountChanged, context);
      }
    }

    PnrSupplementaryAgreements.fields.fieldChanged.add(async (e) => {
      // Изменение суммы договора: Visibility Сумма договора с учетом ДС
      if (e.fieldName == "IsAmountChanged" && e.fieldValue != null)
      {
         this.SetAmountVisibility(e.fieldValue, context);
      }
      // Вид договора: Visibility блоков
      if (e.fieldName == "KindID" && e.fieldValue != null)
      {
        this.SetDKDependenceVisibility(e.fieldValue, context);
        this.ChangeControlsCaption(e.fieldValue, context);
        this.ChangeControlsActiveValidation(e.fieldValue, context);
      }

      // Организация ГК Пионер: заполнение Подписант
      if (e.fieldName == "OrganizationID" && e.fieldValue != null)
      {
          await this.SetSignatory(e.fieldValue, context.card);
      }

      // Вид договора (ДУП): Visibility контролов
      if (e.fieldName == "KindDUPID" && e.fieldValue != null)
      {
          this.SetDK_DUPDependenceVisibility(e.fieldValue, context);
      }

      // Проект: заполняем скрытое поле на карточке (используется в Критериях процесса)
      if (e.fieldName == "ProjectInArchive")
      {
          if(PnrSupplementaryAgreements)
            PnrSupplementaryAgreements.fields.set('IsProjectInArchive', createTypedField(e.fieldValue, DotNetType.Boolean));
      }
    });
  }

  /** установка видимости контролов */
  private SetAmountVisibility(IsAmountChanged, context: ICardUIExtensionContext)
  {
    const amount = context.model.controls.get('Amount');
    const amountSA = context.model.controls.get('AmountSA');
    if (!amount || !amountSA) return;
    amount.controlVisibility = IsAmountChanged ? Visibility.Visible : Visibility.Collapsed;
    amountSA.controlVisibility = IsAmountChanged ? Visibility.Visible : Visibility.Collapsed;
  }

  private ChangeControlsActiveValidation(kindID, context: ICardUIExtensionContext) {
    // Убрать признак обязательного заполнения полей, если Вид договора - С покупателем
    // Сумма ДС
    const amountSA = context.model.controls.get('AmountSA');
    if (amountSA) {
      amountSA.hasActiveValidation = !Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
    }
    // Форма ДС
    const form = context.model.controls.get('Form');
    if (form) {
      form.hasActiveValidation = !Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
    }
  }

  /** Смена caption контролов, зависимых от Вида ДС */
  private ChangeControlsCaption(kindID, context: ICardUIExtensionContext) {
    // Дополнительное соглашение с покупателями
    // Дата заключения -> Дата договора
    const projectDate = context.model.controls.get('ProjectDate');
    if (projectDate) {
      projectDate.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Дата договора" : "Дата заключения";
    }
    // Дата начала -> Дата подписания
    const startDate = context.model.controls.get('StartDate');
    if (startDate) {
      startDate.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Дата подписания" : "Дата начала";
    }
    // Предмет договора -> Заголовок
    const subject = context.model.controls.get('Subject');
    if (subject) {
      subject.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Заголовок" : "Предмет договора";
    }
    // Внешний номер -> Номер договора
    const externalNumber = context.model.controls.get('ExternalNumber');
    if (externalNumber) {
      externalNumber.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Номер договора" : "Внешний номер";
    }
  }

  private SetDKDependenceVisibility(kindID, context: ICardUIExtensionContext)
  {
    const suppAgrDUPBlock = context.model.blocks.get('SuppAgrDUPBlock');
    if (suppAgrDUPBlock) {
      suppAgrDUPBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractDUPID) ? Visibility.Visible : Visibility.Collapsed;
    }
    // const suppAgrCFOBlock = context.model.blocks.get('SuppAgrCFOBlock');
    // if (suppAgrCFOBlock) {
    //   suppAgrCFOBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractCFOID) ? Visibility.Visible : Visibility.Collapsed;
    // }
    const suppAgrWithBuyersBlock = context.model.blocks.get('SuppAgrWithBuyersBlock');
    if (suppAgrWithBuyersBlock) {
      suppAgrWithBuyersBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    }
    const integrationBlock = context.model.blocks.get('IntegrationBlock');
    if (integrationBlock) {
      integrationBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    }

    // Убрать видимость полей, если Вид договора - С покупателем
    // В бюджет
    const isInBudget = context.model.controls.get('IsInBudget');
    if (isInBudget) {
      isInBudget.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Проведен тендер
    const isTenderHeld = context.model.controls.get('IsTenderHeld');
    if (isTenderHeld) {
      isTenderHeld.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // ЦФО
    const CFO = context.model.controls.get('CFO');
    if (CFO) {
      CFO.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Наименование статьи затрат
    const costItem = context.model.controls.get('CostItem');
    if (costItem) {
      costItem.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Тип договора
    const type = context.model.controls.get('Type');
    if (type) {
      type.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Подписант
    const signatory = context.model.controls.get('Signatory');
    if (signatory) {
      signatory.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Внутренний номер
    const internalNumber = context.model.controls.get('InternalNumber');
    if (internalNumber) {
      internalNumber.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Причина заключения ДС
    const reason = context.model.controls.get('Reason');
    if (reason) {
      reason.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Изменение суммы договора
    const isAmountChanged = context.model.controls.get('IsAmountChanged');
    if (isAmountChanged) {
      isAmountChanged.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Сумма ДС (руб.)
    // const amountSA = context.model.controls.get('AmountSA');
    // if (amountSA) {
    //   amountSA.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    // }
    // Сумма аванса (руб.)
    const prepaidExpenseAmount = context.model.controls.get('PrepaidExpenseAmount');
    if (prepaidExpenseAmount) {
      prepaidExpenseAmount.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Валюта расчета
    const settlementCurrency = context.model.controls.get('SettlementCurrency');
    if (settlementCurrency) {
      settlementCurrency.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Ставка НДС
    const VATRate = context.model.controls.get('VATRate');
    if (VATRate) {
      VATRate.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Форма ДС
    const form = context.model.controls.get('Form');
    if (form) {
      form.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Дата окончания
    const endDate = context.model.controls.get('EndDate');
    if (endDate) {
      endDate.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Основной договор
    const mainContract = context.model.controls.get('MainContract');
    if (mainContract) {
      mainContract.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Основной договор заключен до 2019
    const isUntil2019 = context.model.controls.get('IsUntil2019');
    if (isUntil2019) {
      isUntil2019.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Согласование бухгалтерией
    const CRMApprove = context.model.controls.get('CRMApprove');
    if (CRMApprove) {
      CRMApprove.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    }
    
  }

  /** установка видимости контролов, зависимых от Вида договора (ДУП) */
  private SetDK_DUPDependenceVisibility(kindDUPID, context: ICardUIExtensionContext){
    // Доп.согласующие при Вид договора (ДУП) == "Нестроительный"
    const approvingPersons = context.model.controls.get('ApprovingPersons');
    if (approvingPersons) {
        approvingPersons.controlVisibility = Guid.equals(kindDUPID, PnrContractKinds.PnrContractDUPNotBuildingID) ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  private async SetSignatory(organizationID, card: Card) {
    let HeadLegalEntityID, HeadLegalEntityName;
    let request = new CardRequest();
        request.requestType = PnrRequestTypes.GetOrganizationHead;
        request.info["organizationID"] = createTypedField(organizationID, DotNetType.Guid);

    // проверка ответа
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
        HeadLegalEntityID = response.info.HeadLegalEntityID;
        HeadLegalEntityName = response.info.HeadLegalEntityName;
    }
    let pnrSupplementaryAgreements = card.sections.tryGet("PnrSupplementaryAgreements");
    if (HeadLegalEntityID != null && pnrSupplementaryAgreements != null) {
        pnrSupplementaryAgreements.fields.set('SignatoryID', createTypedField(HeadLegalEntityID.$value, DotNetType.Guid))
        pnrSupplementaryAgreements.fields.set('SignatoryName', createTypedField(HeadLegalEntityName.$value, DotNetType.String))
    }
  }
}