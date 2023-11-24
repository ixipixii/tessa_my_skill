﻿using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Files
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<ServerKrPermissionsGetFileVersionsExtension>(
                    new ContainerControlledLifetimeManager())

                .RegisterType<ServerKrPermissionsGetFileContentExtension>(
                    new ContainerControlledLifetimeManager())

                .RegisterType<DocLoadBarcodeGetFileContentExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrVirtualFileGetContentExtension>(new ContainerControlledLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardGetFileVersionsExtension, ServerKrPermissionsGetFileVersionsExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardGetFileContentExtension, ServerKrPermissionsGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardGetFileContentExtension, ServerPathGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton()
                    .WhenFileTypes(DefaultFileTypes.ServerPath))

                .RegisterExtension<ICardGetFileContentExtension, EmbeddedDataGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton()
                    .WhenFileTypes(DefaultFileTypes.EmbeddedData))

                .RegisterExtension<ICardRequestExtension, MailSentRequestExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton()
                    .WhenRequestTypes(CardRequestTypes.MailSent))

                .RegisterExtension<ICardGetFileContentExtension, DocLoadBarcodeGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenFileTypes(DefaultFileTypes.Barcode))

                .RegisterExtension<ICardGetFileContentExtension, KrVirtualFileGetContentExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenFileTypes(DefaultFileTypes.KrVirtualFile))
                ;
        }
    }
}
