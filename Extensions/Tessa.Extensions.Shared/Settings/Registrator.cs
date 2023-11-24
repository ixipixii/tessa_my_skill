using System;
using System.Collections.Generic;
using System.Text;
using Tessa.Platform.Settings;

namespace Tessa.Extensions.Shared.Settings
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ISettingsExtension, PnrSettings>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton())
                ;
        }
    }
}
