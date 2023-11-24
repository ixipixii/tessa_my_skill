using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<WfSatelliteGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfSatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfSatelliteExportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTasksServerGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfGetResolutionVisualizationDataRequestExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfCardMetadataExtension>(new ContainerControlledLifetimeManager(), new InjectionConstructor())
                .RegisterType<WfTaskSatelliteGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteBackupExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteExportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteImportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteGetFileContentExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteGetFileVersionsExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfWorkflowStoreExtension>(new PerResolveLifetimeManager())
                .RegisterType<WfSatelliteImportExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfSatelliteDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfSatelliteRestoreExtension>(new ContainerControlledLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            // Metadata
            extensionContainer
                .RegisterExtension<ICardMetadataExtension, WfCardMetadataExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                ;

            // Get
            extensionContainer
                .RegisterExtension<ICardGetExtension, WfTaskSatelliteGetExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.WfTaskCardTypeID))

                .RegisterExtension<ICardGetExtension, WfSatelliteGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.WfSatelliteTypeID))

                .RegisterExtension<ICardGetExtension, WfSatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))

                .RegisterExtension<ICardGetExtension, WfSatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))

                .RegisterExtension<ICardGetExtension, WfTasksServerGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Default, CardGetMethod.Backup, CardGetMethod.Export))

                .RegisterExtension<ICardGetExtension, WfTaskSatellitePermissionsGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithSingleton()
                    .WhenCardTypes(DefaultCardTypes.WfTaskCardTypeID))

                .RegisterExtension<ICardGetExtension, WfTaskSatelliteBackupExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 6)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Backup))

                .RegisterExtension<ICardGetExtension, WfTaskSatelliteExportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 7)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardGetMethod.Export))
                ;

            // GetFileContent
            extensionContainer
                .RegisterExtension<ICardGetFileContentExtension, WfTaskSatelliteGetFileContentExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.WfTaskCardTypeID))
                ;

            // GetFileVersions
            extensionContainer
                .RegisterExtension<ICardGetFileVersionsExtension, WfTaskSatelliteGetFileVersionsExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.WfTaskCardTypeID))
                ;

            // Store
            extensionContainer
                .RegisterExtension<ICardStoreExtension, WfTaskSatelliteStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.WfTaskCardTypeID))

                .RegisterExtension<ICardStoreExtension, WfWorkflowStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardStoreExtension, WfSatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))

                .RegisterExtension<ICardStoreExtension, WfTaskSatelliteImportExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Import))
                ;

            // Store Task
            extensionContainer
                .RegisterExtension<ICardStoreTaskExtension, WfResolutionStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton()
                    .WhenTaskTypes(WfHelper.ResolutionTaskTypeIDList))
                .RegisterExtension<ICardStoreTaskExtension, WfResolutionCheckSafeLimitStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithSingleton()
                    .WhenTaskTypes(WfHelper.ResolutionTaskTypeIDList))
                ;

            // Delete
            extensionContainer
                .RegisterExtension<ICardDeleteExtension, WfSatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardDeleteExtension, WfSatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))

                .RegisterExtension<ICardDeleteExtension, WfTaskSatelliteDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardDeleteExtension, WfTaskSatelliteRestoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.DeletedTypeID))
                ;

            // Request
            extensionContainer
               .RegisterExtension<ICardRequestExtension, WfGetResolutionVisualizationDataRequestExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(DefaultRequestTypes.GetResolutionVisualizationData))
                ;

            extensionContainer
                //Расширение на выдачу прав по резолюциям Wf
                .RegisterExtension<ITaskPermissionsExtension, WfTasksPermissionsExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithSingleton()
                    .WhenTaskTypes(WfHelper.ResolutionTaskTypeIDList))
                ;        
        }
    }
}
