import { ExtensionContainer, ExtensionStage } from 'tessa/extensions';
import { HideDefaultTilesExtension } from './HideDefaultTilesExtension';

ExtensionContainer.instance.registerExtension({extension: HideDefaultTilesExtension, stage: ExtensionStage.AfterPlatform});
