import { CardUIExtension, IBlockViewModel, ICardUIExtensionContext } from 'tessa/ui/cards';
import { Guid, Visibility } from 'tessa/platform';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { LabelViewModel, FileListViewModel} from 'tessa/ui/cards/controls';
import { IFile } from 'tessa/files';

export class PnrPartnerUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PartnerTypeID)) {
      return;
    }
    let Partners = context.card.sections.tryGet("Partners");
    if (!Partners) return;
    let partnerType = Partners.fields.get('TypeName');
    if (partnerType != null)
    {
      this.SetBlockVisibility(context, partnerType);
    }
    Partners.fields.fieldChanged.add(async (e) => {
      if (e.fieldName == "TypeName" && e.fieldValue != null)
      {
        this.SetBlockVisibility(context, e.fieldValue);
      }
    });
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
  }

  /** Установка видимости контролов, зависимых от Типа контрагента */
  private SetBlockVisibility(context: ICardUIExtensionContext, partnerType)
  {
    const blockLegalEntity = context.model.blocks.get('BlockLegalEntity');
    if (blockLegalEntity) {
      let isLegalEntityOrSoleTrader = partnerType == "$PartnerType_LegalEntity" || partnerType == "$PartnerType_SoleTrader";
      blockLegalEntity.blockVisibility = isLegalEntityOrSoleTrader ? Visibility.Visible : Visibility.Collapsed;
    }
    const blockIndividual = context.model.blocks.get('BlockIndividual');
    if (blockIndividual) {
      let isIndividual = partnerType == "$PartnerType_Individual";
      blockIndividual.blockVisibility = isIndividual ? Visibility.Visible : Visibility.Collapsed;
    }

    // блоки-подсказки перечня категорий файлов по типам КА
    // для Юридического лица
    const fileHelperLegalEntity = context.model.blocks.get('FileHelperLegalEntity');
    if(fileHelperLegalEntity)
    {
      fileHelperLegalEntity.blockVisibility = (partnerType == "$PartnerType_LegalEntity") ? Visibility.Visible : Visibility.Collapsed;
    }
    // для Индивидуального предпринимателя
    const fileHelperSoleTrader = context.model.blocks.get('FileHelperSoleTrader');
    if(fileHelperSoleTrader)
    {
      fileHelperSoleTrader.blockVisibility = (partnerType == "$PartnerType_SoleTrader") ? Visibility.Visible : Visibility.Collapsed;
    }
    // для Физического лица
    const fileHelperIndividual = context.model.blocks.get('FileHelperIndividual');
    if(fileHelperIndividual)
    {
      fileHelperIndividual.blockVisibility = (partnerType == "$PartnerType_Individual") ? Visibility.Visible : Visibility.Collapsed;
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