import { ExtensionContainer, ExtensionStage } from 'tessa/extensions';
import { PnrIncomingStoreExtension } from './PnrIncomingStoreExtension';

ExtensionContainer.instance.registerExtension({ extension: PnrIncomingStoreExtension, stage: ExtensionStage.AfterPlatform, order: 10 });


