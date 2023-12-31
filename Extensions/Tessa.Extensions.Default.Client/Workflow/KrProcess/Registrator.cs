﻿using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    [Registrator]
    public sealed class Registrator: RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<IKrProcessLauncher, KrProcessClientLauncher>(new ContainerControlledLifetimeManager())
                .RegisterType<IKrGlobalTileContainer, KrGlobalTileContainer>(new ContainerControlledLifetimeManager())
                .RegisterType<IKrTileInflater, KrTileInflater>(new ContainerControlledLifetimeManager())
                .RegisterType<IKrTileCommand, KrGlobalTileCommand>(KrTileCommandNames.Global, new ContainerControlledLifetimeManager())
                .RegisterType<IKrTileCommand, KrLocalTileCommand>(KrTileCommandNames.Local, new ContainerControlledLifetimeManager())
                
                .RegisterType<KrCardMetadataExtension>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(typeof(ICardMetadata), typeof(ICardCache)))
                ;
        }

        public override void InitializeExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardMetadataExtension, KrCardMetadataExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                ;
        }
    }
}