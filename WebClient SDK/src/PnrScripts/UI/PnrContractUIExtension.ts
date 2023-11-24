import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { Card } from 'tessa/cards';
import { LabelViewModel } from 'tessa/ui/cards/controls';
import { CardRequest, CardService } from 'tessa/cards/service';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrContractKinds } from '../Shared/PnrContractKinds';

export class PnrContractUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (context.model == null || context.model.card == null) {
        return;
    }
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrContractTypeID)) {
      return;
    }

    let pnrContracts = context.card.sections.tryGet("PnrContracts");
    if (!pnrContracts) return;
    // Есть Вид договора - устанавливаем видимость контролов в зависимости от Вида
    if (pnrContracts.fields.get('KindID') != null) {
        this.SetDKDependenceVisibility(pnrContracts.fields.get('KindID'), context);
        this.ChangeControlsCaption(pnrContracts.fields.get('KindID'), context);
        this.ChangeControlsActiveValidation(pnrContracts.fields.get('KindID'), context);
    }
    if (pnrContracts.fields.get('KindDUPID') != null){
        this.SetDK_DUPDependenceVisibility(pnrContracts.fields.get('KindDUPID'), context);
    }
    
    pnrContracts.fields.fieldChanged.add(async (e) => {
        // Вид договора: Visibility контролов
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
            if(pnrContracts)
                pnrContracts.fields.set('IsProjectInArchive', createTypedField(e.fieldValue, DotNetType.Boolean));
        }
    });

    await this.SetHyperlinkControls(context);
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
    let pnrContracts = card.sections.tryGet("PnrContracts");
    if (HeadLegalEntityID != null && pnrContracts != null) {
        pnrContracts.fields.set('SignatoryID', createTypedField(HeadLegalEntityID.$value, DotNetType.Guid))
        pnrContracts.fields.set('SignatoryName', createTypedField(HeadLegalEntityName.$value, DotNetType.String))
    }
  }

  private ChangeControlsActiveValidation(kindID, context: ICardUIExtensionContext) {
    // Дата проекта/Дата заключения, Заголовок, КА: Договор с покупателями - валидации нет, остальные виды - есть
    const subject = context.model.controls.get('Subject');
    if (subject) {
        subject.hasActiveValidation = !Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
    }
    const partner = context.model.controls.get('Partner');
    if (partner) {
        partner.hasActiveValidation = !Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
    }
    const projectDate = context.model.controls.get('ProjectDate');
    if (projectDate) {
        projectDate.hasActiveValidation = !Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
    }
  }

  /** Смена caption контролов, зависимых от Вида договора */
  private ChangeControlsCaption(kindID, context: ICardUIExtensionContext) {
    // Договор с покупателями
    // Предмет договора -> Заголовок
    const subject = context.model.controls.get('Subject');
    if (subject) {
        subject.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Заголовок" : "Предмет договора";
    }
    // Дата заключения -> Дата проекта
    const projectDate = context.model.controls.get('ProjectDate');
    if (projectDate) {
        projectDate.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Дата договора" : "Дата заключения";
    }
    // Внешний номер -> Номер договора
    const externalNumber = context.model.controls.get('ExternalNumber');
    if (externalNumber) {
        externalNumber.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Номер договора" : "Внешний номер";
    }
    // Дата начала -> Дата подписания
    const startDate = context.model.controls.get('StartDate');
    if (startDate) {
        startDate.caption = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? "Дата подписания" : "Дата начала";
    }
  }

  /** установка видимости контролов, зависимых от Вида договора */
  private SetDKDependenceVisibility(kindID, context: ICardUIExtensionContext) {
    // Договор с покупателями
    // Скрываем
    // Внутренний номер
    const internalNumber = context.model.controls.get('InternalNumber');
    if (internalNumber) {
        internalNumber.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Ставка НДС
    const VATRate = context.model.controls.get('VATRate');
    if (VATRate) {
        VATRate.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // ЦФО
    const cfo = context.model.controls.get('CFO');
    if (cfo) {
        cfo.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Наименование статьи затрат
    const costItem = context.model.controls.get('CostItem');
    if (costItem) {
        costItem.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Сумма договора (руб.)
    const amount = context.model.controls.get('Amount');
    if (amount) {
        amount.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
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
    // В бюджете
    const isInBudget = context.model.controls.get('IsInBudget');
    if (isInBudget) {
        isInBudget.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }  
    // Проведен тендер
    const isTenderHeld = context.model.controls.get('IsTenderHeld');
    if (isTenderHeld) {
        isTenderHeld.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }  
    // Тип договора
    const type = context.model.controls.get('Type');
    if (type) {
        type.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }  
    // Отсрочка платежа (раб.дн.)
    const defermentPayment = context.model.controls.get('DefermentPayment');
    if (defermentPayment) {
        defermentPayment.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }  
    // Планируемая дата актирования
    const plannedActDate = context.model.controls.get('PlannedActDate');
    if (plannedActDate) {
        plannedActDate.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    } 
    // Форма договора
    const form = context.model.controls.get('Form');
    if (form) {
        form.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    } 
    // Дата окончания
    const endDate = context.model.controls.get('EndDate');
    if (endDate) {
        endDate.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    } 
    // Дата окончания
    const signatory = context.model.controls.get('Signatory');
    if (signatory) {
        signatory.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Контролы Требует согласования, Номер квартиры, Статус действия, Срочность
    const additionalControlsBlock = context.model.blocks.get('AdditionalControlsBlock');
    if (additionalControlsBlock) {
        additionalControlsBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    }
    // Требуется согласование
    const isRequiresApproval = context.model.controls.get('IsRequiresApproval');
    if (isRequiresApproval) {
        isRequiresApproval.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // Вид договора 1С
    const kind1C = context.model.controls.get('Kind1C');
    if (kind1C) {
        kind1C.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Collapsed : Visibility.Visible;
    }
    // В блоке Интеграция НСИ
    // Статус договора в CRM
    // const crmContractStatus = context.model.controls.get('CRMContractStatus');
    // if (crmContractStatus) {
    //     crmContractStatus.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    // }
    // Номер договора для МДМ
    const mdmContractNumber = context.model.controls.get('MDMContractNumber');
    if (mdmContractNumber) {
        mdmContractNumber.controlVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractWithBuyersID) ? Visibility.Visible : Visibility.Collapsed;
    }

    const contractDUPBlock = context.model.blocks.get('ContractDUPBlock');
    if (contractDUPBlock) {
        contractDUPBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractDUPID) ? Visibility.Visible : Visibility.Collapsed;
    }
    // const contractCFOBlock = context.model.blocks.get('ContractCFOBlock');
    // if (contractCFOBlock) {
    //     contractCFOBlock.blockVisibility = Guid.equals(kindID, PnrContractKinds.PnrContractCFOID) ? Visibility.Visible : Visibility.Collapsed;
    // }
  }

  /** установка видимости контролов, зависимых от Вида договора (ДУП) */
  private SetDK_DUPDependenceVisibility(kindDUPID, context: ICardUIExtensionContext){
    // Доп.согласующие при Вид договора (ДУП) == "Нестроительный"
    const approvingPersons = context.model.controls.get('ApprovingPersons');
    if (approvingPersons) {
        approvingPersons.controlVisibility = Guid.equals(kindDUPID, PnrContractKinds.PnrContractDUPNotBuildingID) ? Visibility.Visible : Visibility.Collapsed;
    }
  }

    /** Установка гиперссылки в контрол. */
    private async SetHyperlinkControls(context: ICardUIExtensionContext)
    {
        // Ссылка на карточку договора в CRM
        let pnrContracts = context.card.sections.tryGet("PnrContracts");
        const contractLinkCrmControl = context.model.controls.get('ContractLinkCrmControl') as LabelViewModel;
        if (contractLinkCrmControl && pnrContracts && pnrContracts.fields.get('LinkCardCRM') != null) {
            contractLinkCrmControl.text = pnrContracts.fields.get('LinkCardCRM');
            contractLinkCrmControl.hyperlink = true;
            contractLinkCrmControl.controlVisibility = Visibility.Visible;
            contractLinkCrmControl.onClick = () => {
                if (pnrContracts) {
                    window.open(pnrContracts.fields.get('LinkCardCRM'));
                }
            };
        }
        const contractLinkCardControl = context.model.controls.get('ContractLinkCardControl') as LabelViewModel;
        if (contractLinkCardControl && pnrContracts && pnrContracts.fields.get('HyperlinkCard') != null) {
            contractLinkCardControl.text = pnrContracts.fields.get('HyperlinkCard');
            contractLinkCardControl.hyperlink = true;
            contractLinkCardControl.controlVisibility = Visibility.Visible;
            contractLinkCardControl.onClick = () => {
                if (pnrContracts) {
                    window.open(pnrContracts.fields.get('HyperlinkCard'));
                }
            };
        }
    }
}