import { ExtensionContainer, ExtensionStage } from 'tessa/extensions';
import { DocxToPdfFileExtension } from './DocxToPdfFileExtension';

ExtensionContainer.instance.registerExtension({extension: DocxToPdfFileExtension, stage: ExtensionStage.BeforePlatform});