using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<KrSatelliteGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSatelliteExportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSatelliteImportExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrSecondarySatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSecondarySatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSecondarySatelliteImportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSecondarySatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSecondarySatelliteExportExtension>(new ContainerControlledLifetimeManager())
                
                .RegisterType<KrDialogSatelliteNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDialogSatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDialogSatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDialogSatelliteImportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDialogSatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDialogSatelliteExportExtension>(new ContainerControlledLifetimeManager())
                ;
        }

        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardGetExtension, KrSatelliteGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrSatelliteTypeID))
                .RegisterExtension<ICardDeleteExtension, KrSatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, KrSatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))
                .RegisterExtension<ICardDeleteExtension, KrSatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))
                .RegisterExtension<ICardStoreExtension, KrSatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))
                .RegisterExtension<ICardGetExtension, KrSatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))


                .RegisterExtension<ICardDeleteExtension, KrSecondarySatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, KrSecondarySatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))
                .RegisterExtension<ICardStoreExtension, KrSecondarySatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))
                .RegisterExtension<ICardDeleteExtension, KrSecondarySatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))
                .RegisterExtension<ICardGetExtension, KrSecondarySatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))

                
                .RegisterExtension<ICardDeleteExtension, KrDialogSatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, KrDialogSatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))
                .RegisterExtension<ICardStoreExtension, KrDialogSatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))
                .RegisterExtension<ICardDeleteExtension, KrDialogSatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))
                .RegisterExtension<ICardGetExtension, KrDialogSatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))
                .RegisterExtension<ICardNewExtension, KrDialogSatelliteNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithSingleton()
                    .WhenAnyCardType())

                ;
        }
    }
}