import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrServiceNoteTypes } from '../Shared/PnrServiceNoteTypes';
import { CardRequest, CardService } from 'tessa/cards/service';
import { DateTimeViewModel } from 'tessa/ui/cards/controls';
import Moment from 'moment';

export class PnrServiceNoteUIExtension extends CardUIExtension {
  public async initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrServiceNoteTypeID)) {
      return;
    }
    let PnrServiceNote = context.card.sections.tryGet("PnrServiceNote");
    if (!PnrServiceNote) return;
    // Создание карточки
    if (context.card.storeMode === CardStoreMode.Insert)
    {
        const regDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
        if (!regDateControl) return;
        // Заполнить поле Дата проекта текущей датой
        regDateControl.selectedDate = Moment(Date.now());

      let dci = context.card.sections.tryGet("DocumentCommonInfo");
      if (dci != null) {
        // начальная инициализация поля Подразделение по автору
        await this.SetUserDepartmentInfo(dci.fields.get('AuthorID'), context, true);
      }
    }
    else {
      // Открытие карточки. Есть Тип служебной записки - устанавливаем видимость блоков в зависимости от Типа
      this.SetNTDependenceVisibility(PnrServiceNote.fields.get('ServiceNoteTypeID'), context);
    }

    PnrServiceNote.fields.fieldChanged.add(async (e) => {
      // Тип служебной записки: Visibility блоков
      if (e.fieldName === "ServiceNoteTypeID" && e.fieldValue != null)
      {
         this.SetNTDependenceVisibility(e.fieldValue, context);
         this.ChangeControlsActiveValidation(e.fieldValue, context);
      }
      // Организация ГК Пионер: заполнение Индекс ЮЛ (но он далее нигде не используется)
      // if (e.fieldName == "OrganizationID" && e.fieldValue != null)
      // {
      //   await this.SetLegalEntityIndex(e.fieldValue, context);
      // }
      // Адресат: подчитываем его подразделение
      if (e.fieldName === "DestinationID" && e.fieldValue != null)
      {
        await this.SetUserDepartmentInfo(e.fieldValue, context, false);
      }
    });
  }

  // private async SetLegalEntityIndex(organizationID, context: ICardUIExtensionContext)
  // {
  //   // request к организации получить индекс ЮЛ
  //   let LegalEntityIdx;
  //   let request = new CardRequest();
  //       request.requestType = PnrRequestTypes.GetIndexLegalEntityRequestTypeID;
  //       request.info["organizationID"] = createTypedField(organizationID, DotNetType.Guid);
  //   // проверка ответа
  //   let response = await CardService.instance.request(request);
  //   if (response.validationResult.isSuccessful) {
  //       LegalEntityIdx = response.info.LegalEntityIdx;
  //   }
  //   let PnrServiceNote = context.card.sections.tryGet("PnrServiceNote");
  //   if (LegalEntityIdx != null && PnrServiceNote != null) {
  //     PnrServiceNote.fields.set('LegalEntityIndexIdx', createTypedField(LegalEntityIdx.$value, DotNetType.String))
  //   }
  // }

  /** установка видимости блоков, зависимых от Типа служебной записки */
  private SetNTDependenceVisibility(serviceNoteTypeID, context: ICardUIExtensionContext)
  {
    const personnelBlock = context.model.blocks.get('PersonnelBlock');
    if (personnelBlock) {
      personnelBlock.blockVisibility = serviceNoteTypeID === PnrServiceNoteTypes.Personnel ? Visibility.Visible : Visibility.Collapsed;
    }
    const conclusionContractsBlock = context.model.blocks.get('ConclusionContractsBlock');
    if (conclusionContractsBlock) {
      conclusionContractsBlock.blockVisibility = serviceNoteTypeID === PnrServiceNoteTypes.ConclusionContracts ? Visibility.Visible : Visibility.Collapsed;
    }
    const conclusionContractsDUPBlock = context.model.blocks.get('ConclusionContractsDUPBlock');
    if (conclusionContractsDUPBlock) {
      conclusionContractsDUPBlock.blockVisibility = serviceNoteTypeID === PnrServiceNoteTypes.ConclusionContractsDUP ? Visibility.Visible : Visibility.Collapsed;
    }
    const financialActivitiesBlock = context.model.blocks.get('FinancialActivitiesBlock');
    if (financialActivitiesBlock) {
      financialActivitiesBlock.blockVisibility = serviceNoteTypeID === PnrServiceNoteTypes.FinancialActivities ? Visibility.Visible : Visibility.Collapsed;
    }
    const workQuestionsBlock = context.model.blocks.get('WorkQuestionsBlock');
    if (workQuestionsBlock) {
      workQuestionsBlock.blockVisibility = serviceNoteTypeID === PnrServiceNoteTypes.WorkQuestions ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  private ChangeControlsActiveValidation(serviceNoteTypeID, context: ICardUIExtensionContext)
        {
          let contractsThemeControl = context.model.controls.get("ConclusionContractsTheme");
          let activitiesThemeControl = context.model.controls.get("FinancialActivitiesTheme");
            if (contractsThemeControl && activitiesThemeControl)
            {
              contractsThemeControl.isRequired = serviceNoteTypeID === PnrServiceNoteTypes.ConclusionContracts;
              activitiesThemeControl.isRequired = serviceNoteTypeID === PnrServiceNoteTypes.FinancialActivities;
              contractsThemeControl.notifyUpdateValidation();
              activitiesThemeControl.notifyUpdateValidation();
            }
        }

  private async SetUserDepartmentInfo(authorID, context: ICardUIExtensionContext, isInitiator: boolean)
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

    let PnrServiceNote = context.card.sections.tryGet("PnrServiceNote");
    if (DepartmentID != null && DepartmentName != null && DepartmentIdx != null && PnrServiceNote != null) {
      if (isInitiator) {
        PnrServiceNote.fields.set('DepartmentID', createTypedField(DepartmentID.$value, DotNetType.Guid));
        PnrServiceNote.fields.set('DepartmentName', createTypedField(DepartmentName.$value, DotNetType.String));
        PnrServiceNote.fields.set('DepartmentIdx', createTypedField(DepartmentIdx.$value, DotNetType.String));
      }
      else {
        PnrServiceNote.fields.set('DestinationDepartmentID', createTypedField(DepartmentID.$value, DotNetType.Guid));
        PnrServiceNote.fields.set('DestinationDepartmentName', createTypedField(DepartmentName.$value, DotNetType.String));
        PnrServiceNote.fields.set('DestinationDepartmentIdx', createTypedField(DepartmentIdx.$value, DotNetType.String));
      }
    }
  }
}