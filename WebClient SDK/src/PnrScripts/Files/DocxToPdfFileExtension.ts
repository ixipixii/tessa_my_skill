import { FileExtension, IFileExtensionContext } from 'tessa/ui/files';
import { Card } from 'tessa/cards';
import { UIContext, MenuAction, showMessage, showLoadingOverlay } from 'tessa/ui';
import { ICardEditorModel } from 'tessa/ui/cards';
import { CardRequest, CardService } from 'tessa/cards/service';
import { FileViewModel } from 'tessa/ui/cards/controls';
import { DotNetType, createTypedField } from 'tessa/platform';
import { PnrRequestTypes } from '../Shared/PnrRequestTypes';

export class DocxToPdfFileExtension extends FileExtension {
  public async openingMenu(context: IFileExtensionContext) {
    let uiContext = UIContext.current;
    let editor = uiContext.cardEditor;
    let card: Card = context.control.model.card;
    // Добавляем пункт меню для получения файлов
    context.actions.push(
        new MenuAction(
          'DocxToPdf',
          'Конвертировать в pdf',
          "ta icon-thin-427",
          async () => await this.ConvertDocxFileToPdfMultiple(card, context.files, editor),
          null,
          false
      ));
  }

  private async ConvertDocxFileToPdfMultiple(card: Card, files: ReadonlyArray<FileViewModel>, editor: ICardEditorModel | null) {
    if (!editor) return;
    await showLoadingOverlay(async () => {
        let cardID = card.id;
        let notConvertedDocxFiles = '';
        let notSavedDocxFiles = '';
        for (let i = 0; i < files.length; i++) {
            let versionsCount = files[i].model.versions.length;
            let versionState = files[i].model.versions[versionsCount - 1].state;
            if (versionState == 0) {
                if (notSavedDocxFiles != null && notSavedDocxFiles.length > 0) {
                    notSavedDocxFiles += ', ';
                }
                else {
                    notSavedDocxFiles += (files[i].model.name + ' ');
                }
            }
        }
        if (notSavedDocxFiles != null && notSavedDocxFiles.length > 0) {
            showMessage('Файлы: ' + notSavedDocxFiles + ' добавлены в карточку, но не сохранены. Сохраните карточку перед конвертацией файлов.', 'Конвертация в pdf');
            return;
        }
    
        for (let i = 0; i < files.length; i++) {
            let versionsCount = files[i].model.versions.length;
            let versionRowID = files[i].model.versions[versionsCount - 1].id;
    
            let request = new CardRequest();
                request.requestType = PnrRequestTypes.GetPdfFromDocxTypeID;
                request.info["versionRowID"] = createTypedField(versionRowID, DotNetType.Guid);
                request.info["cardID"] = createTypedField(cardID, DotNetType.Guid);
                request.info["fileName"] = createTypedField(files[i].model.name, DotNetType.String);
    
            // проверка ответа
            let response = await CardService.instance.request(request);
            if (!response.validationResult.isSuccessful || !response.info.ConvertFileResult) {
                if (notConvertedDocxFiles != null && notConvertedDocxFiles.length > 0) {
                    notConvertedDocxFiles += ', ';
                }
                else {
                    notConvertedDocxFiles += (files[i].model.name + ' ');
                }
            }
        }
    
        if (notConvertedDocxFiles != null && notConvertedDocxFiles.length > 0) {
            showMessage('Конвертация файлов: ' + notConvertedDocxFiles + ' в pdf завершилась с ошибкой.', 'Конвертация в pdf');
            return;
        }
    });
    await editor.refreshCard(editor.context);
  }
}