using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Forum.Satellite;
using Tessa.Extensions.Default.Shared.Settings;
using Tessa.Forums;
using Tessa.Platform.Initialization;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Forum
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<InjectForumCardMetadataExtension>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteGetFileContentExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<IForumPermissionsProvider, KrForumPermissionsProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteExportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteImportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ForumSatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                
                .RegisterWorkplaceInitializationRule<ForumWorkplaceInitialization>(new PerResolveLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardMetadataExtension, InjectForumCardMetadataExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, ForumGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, ForumSatelliteGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(ForumHelper.ForumSatelliteTypeID))
                .RegisterExtension<ICardStoreExtension, ForumSatelliteStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(ForumHelper.ForumSatelliteTypeID))
                .RegisterExtension<ICardGetFileContentExtension, ForumSatelliteGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(ForumHelper.ForumSatelliteTypeID))
                .RegisterExtension<ICardStoreExtension, ForumSatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))
                .RegisterExtension<ICardGetExtension, ForumSatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))
                .RegisterExtension<ICardGetExtension, ForumSatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))
                .RegisterExtension<ICardDeleteExtension, ForumSatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardDeleteExtension, ForumSatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))
                ;

        }
    }

}
