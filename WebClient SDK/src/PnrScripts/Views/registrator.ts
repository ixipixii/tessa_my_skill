import { ExtensionContainer, ExtensionStage } from 'tessa/extensions';

import { PnrRefresh, PnrRefreshTreeItemExtension } from './PnrRefresh';

ExtensionContainer.instance.registerExtension({
  extension:PnrRefresh,
  stage: ExtensionStage.AfterPlatform,
  singleton: true
});

ExtensionContainer.instance.registerExtension({
  extension:PnrRefreshTreeItemExtension,
  stage: ExtensionStage.AfterPlatform,
  singleton: true
});