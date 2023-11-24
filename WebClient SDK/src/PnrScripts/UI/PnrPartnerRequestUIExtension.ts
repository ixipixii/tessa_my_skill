import { CardUIExtension, IBlockViewModel, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, createTypedField, DotNetType, Visibility } from 'tessa/platform';
// import { Guid, Visibility } from 'tessa/platform';
import { CardStoreMode } from 'tessa/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
// import { PnrCountries } from '../Shared/PnrCountries';
import { PnrSpecialSigns } from '../Shared/PnrSpecialSigns';
import { DateTimeViewModel, LabelViewModel, FileListViewModel} from 'tessa/ui/cards/controls';
import { CardRequest, CardService } from 'tessa/cards/service';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';
import { PnrPartnerRequestsTypes } from '../Shared/PnrPartnerRequestsTypes';
import { IFile } from 'tessa/files';
// import Moment from 'moment';

export class PnrPartnerRequestUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrPartnerRequestTypeID)) {
      return;
    }
    let PnrPartnerRequests = context.card.sections.tryGet("PnrPartnerRequests");
    if (!PnrPartnerRequests) return;

    // Создание карточки
    if (context.card.storeMode === CardStoreMode.Insert)
    {
      const projectDateControl = context.model.controls.get('RegistrationDate') as DateTimeViewModel;
      if (!projectDateControl) return;
    }

    // Есть Тип заявки - устанавливаем видимость Контрагент, зависимого от него
    if (PnrPartnerRequests.fields.get('RequestTypeID') != null) {
      this.SetPartnerVisibility(PnrPartnerRequests.fields.get('RequestTypeID'), context);
    }

    // Есть Тип контрагента - устанавливаем видимость контролов, зависимости от него
    // if (PnrPartnerRequests.fields.get('TypeID') != null) {
    //   this.SetPTDependenceVisibility(PnrPartnerRequests.fields.get('TypeID'), context);
    // }
    const typeID = PnrPartnerRequests.fields.get('TypeID');
    this.SetPTDependenceVisibility(typeID, context);

    // валидатор для КПП
    const kpp = context.model.controls.get('KPP');
    if (kpp != null) {
      kpp.validationFunc = control => control != null && control.hasEmptyValue ? "Поле не заполнено." : null;
    }
    // валидатор для ИНН
    const inn = context.model.controls.get('INN');
    if (inn != null) {
      inn.validationFunc = control => control != null && control.hasEmptyValue ? "Поле не заполнено." : null;
    }

    const fileListModel = context.model.controls.get('Files') as FileListViewModel;

    if (fileListModel != null){
        fileListModel.files.forEach(file => {
            this.ChangeHelperControlVisibility(context, file.model, Visibility.Collapsed);
          });
          fileListModel.containerFileAdded.add( (e) => {
            this.ChangeHelperControlVisibility(context, e.file, Visibility.Collapsed);
          });
          fileListModel.containerFileRemoving.add( (e) => {
            this.ChangeHelperControlVisibility(context, e.file, Visibility.Visible);
          });
      }

    PnrPartnerRequests.fields.fieldChanged.add(async (e) => {
      // Тип контрагента: Visibility контролов
      //if (e.fieldName == "TypeID" && e.fieldValue != null)
      if (e.fieldName == "TypeID")
      {
        this.SetPTDependenceVisibility(e.fieldValue, context);
        this.SetIndividualVisibility(context);
        this.SetNeedFieldValidation(context);
      }
      // Тип заявки: установка видимости и доступности полей
      if (e.fieldName == "RequestTypeID" && e.fieldValue != null)
      {
        this.SetPTDependenceVisibility(typeID, context);
        this.SetPartnerVisibility(e.fieldValue, context);
        this.SetIndividualVisibility(context);
      }

      // Контрагент: подчитываем данные по контрагенту
      if (e.fieldName == "PartnerID" && e.fieldValue != null)
      {
        let request = new CardRequest();
        request.requestType = PnrRequestTypes.PartnerInfo;
        request.info["partnerID"] = createTypedField(e.fieldValue, DotNetType.Guid);
        await this.GetPartnerInfo(request, context);
      }

      // Особый признак контрагента: установка необходимости валидации
      if (e.fieldName == "SpecialSignID" && e.fieldValue != null)
      {
        this.SetNeedFieldValidation(context);
      }

     // Нерезидент: установка необходимости валидации
     if (e.fieldName == "NonResidentID" && e.fieldValue != null)
     {
        this.SetNeedFieldValidation(context);
     }
    });
  }

  /** установка видимости контролов, зависимых от Тип контрагента */
  private SetPTDependenceVisibility(typeID, context: ICardUIExtensionContext)
  {
    let requestTypeID = null;
    let PnrPartnerRequests = context.card.sections.tryGet("PnrPartnerRequests");
    if (PnrPartnerRequests) {
      // тип заявки
      requestTypeID = PnrPartnerRequests.fields.get('RequestTypeID');
    }

    // ИНН
    const inn = context.model.controls.get('INN');
    if (inn) {
      inn.controlVisibility = (typeID === 1 || typeID === 3) ? Visibility.Visible : Visibility.Collapsed;
    }
    // КПП
    const kpp = context.model.controls.get('KPP');
    if (kpp) {
      kpp.controlVisibility = (typeID === 1) ? Visibility.Visible : Visibility.Collapsed;
    }
    // ОГРН
    const ogrn = context.model.controls.get('OGRN');
    if (ogrn) {
      ogrn.controlVisibility = (typeID === 3) ? Visibility.Visible : Visibility.Collapsed;
    }
    // День рождения
    const birthday = context.model.controls.get('Birthday');
    if (birthday) {
      birthday.controlVisibility = (typeID === 2) ? Visibility.Visible : Visibility.Collapsed;
    }
    // Страна регистрации (всегда скрыта в режиме Согласования КА)
    const countryRegistration = context.model.controls.get('CountryRegistration');
    if (countryRegistration) {
      countryRegistration.controlVisibility = ((typeID === 1 || typeID === 2) && requestTypeID !== 1) ? Visibility.Visible : Visibility.Collapsed;
    }

    // блоки-подсказки перечня категорий файлов по типам КА
    // для Юридического лица
    const fileHelperLegalEntity = context.model.blocks.get('FileHelperLegalEntity');
    if(fileHelperLegalEntity)
    {
      fileHelperLegalEntity.blockVisibility = (typeID === 1 && requestTypeID === PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
    }
    // для Индивидуального предпринимателя
    const fileHelperSoleTrader = context.model.blocks.get('FileHelperSoleTrader');
    if(fileHelperSoleTrader)
    {
      fileHelperSoleTrader.blockVisibility = (typeID === 3 && requestTypeID === PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
    }
    // для Физического лица
    const fileHelperIndividual = context.model.blocks.get('FileHelperIndividual');
    if(fileHelperIndividual)
    {
      fileHelperIndividual.blockVisibility = (typeID === 2 && requestTypeID === PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  /** видимость полей и блоков, зависимых от Тип заявки */
  private SetPartnerVisibility(typeID, context: ICardUIExtensionContext)
  {
    // Видимость Контрагент
    const partner = context.model.controls.get('Partner');
    if (partner)
    {
      partner.controlVisibility = (typeID === 1) ? Visibility.Visible : Visibility.Collapsed;
    }

    // Страна регистрации скрыта в режиме "Согласование КА"
    const countryRegistration = context.model.controls.get('CountryRegistration');
    if (countryRegistration)
    {
      countryRegistration.controlVisibility = (typeID === 1) ? Visibility.Collapsed : Visibility.Visible;
    }

    // Disable всех полей блока КА в режиме "Согласование КА"
    const block = context.model.blocks.get('NewPartnerBlock');
    if (block)
    {
      block.controls.forEach(element => {
        element.isReadOnly = (typeID === 1);
      });
    }
    // Особый признак контрагента - должно быть доступно к выбору в режиме "Согласование КА"
    const specialSign = context.model.controls.get('SpecialSign');
    if (specialSign)
    {
      specialSign.isReadOnly = false;
    }

    // Требует согласования КА - скрыто в режиме Согласование КА
    const requiresApprovalCA = context.model.controls.get('RequiresApprovalCA');
    if(requiresApprovalCA)
    {
      requiresApprovalCA.controlVisibility = (typeID === 1) ? Visibility.Collapsed : Visibility.Visible;
    }

    // Файлы - скрыто при Тип заявки - Создание нового контрагента
    const files = context.model.controls.get('Files');
    if(files)
    {
      files.controlVisibility = (typeID === PnrPartnerRequestsTypes.CreateID) ? Visibility.Collapsed : Visibility.Visible;
    }
  }

  // Видимость блока с паспортными данными физического лица
  private SetIndividualVisibility(context: ICardUIExtensionContext)
  {
    let PnrPartnerRequests = context.card.sections.tryGet("PnrPartnerRequests");
    if (!PnrPartnerRequests) return;
    let partnerTypeID = PnrPartnerRequests.fields.get('TypeID');
    let requestTypeID = PnrPartnerRequests.fields.get('RequestTypeID');
    const blockIndividual = context.model.blocks.get('BlockIndividual');
    if (blockIndividual) {
      blockIndividual.blockVisibility = partnerTypeID == 2 && requestTypeID == 0 ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  private SetNeedFieldValidation(context: ICardUIExtensionContext)
  {
      let PnrPartnerRequests = context.card.sections.tryGet("PnrPartnerRequests");
      const kpp = context.model.controls.get('KPP');
      const inn = context.model.controls.get('INN');
      if (!PnrPartnerRequests || !kpp || !inn) return;

      // Тип контрагента
      let typeID = PnrPartnerRequests.fields.get('TypeID');
      // Особый признак контрагента
      let specialSignID = PnrPartnerRequests.fields.get('SpecialSignID');
      // Нерезидент
      let nonResidentID = PnrPartnerRequests.fields.get('NonResidentID');

      // Особый признак контрагента != Гос.органы && Нерезидент != Да
      let isNeedValidation = (specialSignID !== 2) && (nonResidentID !== 1);

      // Валидация КПП при: тип ЮЛ && Особый признак контрагента != Гос.органы && Нерезидент != Да
      kpp.hasActiveValidation = (typeID === 1) && isNeedValidation;
      kpp.isRequired = kpp.hasActiveValidation;
      kpp.notifyUpdateValidation();

      // Валидация ИНН при: (тип ЮЛ || ИП) && Особый признак контрагента != Гос.органы && Нерезидент != Да
      inn.hasActiveValidation = (typeID === 1 || typeID === 3) && isNeedValidation;
      inn.isRequired = inn.hasActiveValidation;
      inn.notifyUpdateValidation();
  }

  private async GetPartnerInfo(request: CardRequest, context: ICardUIExtensionContext)
  {
    let PnrPartnerRequests = context.card.sections.tryGet("PnrPartnerRequests");
    if (!PnrPartnerRequests) return;
    let response = await CardService.instance.request(request);
    if (response.validationResult.isSuccessful) {
      let Name = response.info.Name;
      let FullName = response.info.FullName;
      let TypeID = response.info.TypeID;
      let TypeName = response.info.TypeName;
      let INN = response.info.INN;
      let KPP = response.info.KPP;
      let OGRN = response.info.OGRN;
      let comment = response.info.Comment;

      if (TypeID != null && TypeName != null) {
        PnrPartnerRequests.fields.set('TypeID', createTypedField(TypeID.$value, DotNetType.Int32));
        PnrPartnerRequests.fields.set('TypeName', createTypedField(TypeName.$value, DotNetType.String));
      }
      if (Name != null) {
        PnrPartnerRequests.fields.set('ShortName', createTypedField(Name.$value, DotNetType.String));
      }
      if (FullName != null) {
        PnrPartnerRequests.fields.set('FullName', createTypedField(FullName.$value, DotNetType.String));
      }

      let SpecialSignID = response.info.SpecialSignID;
      let SpecialSignName = response.info.SpecialSignName;
      if (SpecialSignID != null && SpecialSignName != null) {
        PnrPartnerRequests.fields.set('SpecialSignID', createTypedField(SpecialSignID.$value, DotNetType.Int32));
        PnrPartnerRequests.fields.set('SpecialSignName', createTypedField(SpecialSignName.$value, DotNetType.String));
      } else {
        PnrPartnerRequests.fields.set('SpecialSignID', createTypedField(PnrSpecialSigns.NoID, DotNetType.Int32));
        PnrPartnerRequests.fields.set('SpecialSignName', createTypedField(PnrSpecialSigns.NoName, DotNetType.String));
      }

      let NonResident = response.info.NonResident;
      if (NonResident != null) {
        PnrPartnerRequests.fields.set('NonResident', createTypedField(NonResident.$value, DotNetType.Boolean));
      }

      if (INN != null) {
        PnrPartnerRequests.fields.set('INN', createTypedField(INN.$value, DotNetType.String));
      }
      if (KPP != null) {
        PnrPartnerRequests.fields.set('KPP', createTypedField(KPP.$value, DotNetType.String));
      }

      let CountryRegistrationID = response.info.CountryRegistrationID;
      let CountryRegistrationName = response.info.CountryRegistrationName;
      if (CountryRegistrationID != null && CountryRegistrationName != null) {
        PnrPartnerRequests.fields.set('CountryRegistrationID', createTypedField(CountryRegistrationID.$value, DotNetType.Guid));
        PnrPartnerRequests.fields.set('CountryRegistrationName', createTypedField(CountryRegistrationName.$value, DotNetType.String));
      }

      if (comment != null) {
        PnrPartnerRequests.fields.set('Comment', createTypedField(comment.$value, DotNetType.String));
      }

      let IdentityDocument = response.info.IdentityDocument;
      if(IdentityDocument != null) {
        PnrPartnerRequests.fields.set('IdentityDocument', createTypedField(IdentityDocument.$value, DotNetType.String));
      }

      let IdentityDocumentKind = response.info.IdentityDocumentKind;
      if(IdentityDocumentKind != null) {
        PnrPartnerRequests.fields.set('IdentityDocumentKind', createTypedField(IdentityDocumentKind.$value, DotNetType.String));
      }

      let IdentityDocumentIssueDate = response.info.IdentityDocumentIssueDate;
      if(IdentityDocumentIssueDate != null) {
        PnrPartnerRequests.fields.set('IdentityDocumentIssueDate', createTypedField(IdentityDocumentIssueDate.$value, DotNetType.DateTime));
      }

      let IdentityDocumentSeries = response.info.IdentityDocumentSeries;
      if(IdentityDocumentSeries != null) {
        PnrPartnerRequests.fields.set('IdentityDocumentSeries', createTypedField(IdentityDocumentSeries.$value, DotNetType.String));
      }

      let IdentityDocumentNumber = response.info.IdentityDocumentNumber;
      if(IdentityDocumentNumber != null) {
        PnrPartnerRequests.fields.set('IdentityDocumentNumber', createTypedField(IdentityDocumentNumber.$value, DotNetType.String));
      }

      let IdentityDocumentIssuedBy = response.info.IdentityDocumentIssuedBy;
      if(IdentityDocumentIssuedBy != null) {
        PnrPartnerRequests.fields.set('IdentityDocumentIssuedBy', createTypedField(IdentityDocumentIssuedBy.$value, DotNetType.String));
      }

      if (OGRN != null) {
        PnrPartnerRequests.fields.set('OGRN', createTypedField(OGRN.$value, DotNetType.String));
      }
    }
  }

  private ChangeHelperControlVisibility(context: ICardUIExtensionContext, file: IFile, visibility: Visibility)
        {
            let currentHelpBlock = this.GetCurrentHelperBlock(context);
            if(currentHelpBlock === null) return;
            currentHelpBlock.controls.forEach(control => {
              if (control.cardTypeControl.type.name === "Label") {
                  if (file.category != null && (control as LabelViewModel).text.includes(file.category.caption)) {
                      control.controlVisibility = visibility;
                  }
              }
            });
        }

  private GetCurrentHelperBlock(context: ICardUIExtensionContext): IBlockViewModel | null {
        let legalEntityBlock = context.model.blocks.get('FileHelperLegalEntity');
        let soleTraderBlock = context.model.blocks.get('FileHelperSoleTrader');
        let individualBlock = context.model.blocks.get('FileHelperIndividual');
        if (legalEntityBlock!.blockVisibility === Visibility.Visible) {
            return legalEntityBlock as IBlockViewModel;
        }
        else if (soleTraderBlock!.blockVisibility === Visibility.Visible) {
            return soleTraderBlock as IBlockViewModel;
        }
        else if (individualBlock!.blockVisibility === Visibility.Visible) {
            return individualBlock as IBlockViewModel;
        }
        else {
            return null;
        }
    }
}