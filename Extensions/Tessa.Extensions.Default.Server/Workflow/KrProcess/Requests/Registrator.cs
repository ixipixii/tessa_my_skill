﻿using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer

                .RegisterType<KrCheckStageRowModifiedStoreExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrCardNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrCardGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrCardStoreExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrLocalTilesNewGetExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrProcessWorkflowStoreExtension>(new PerResolveLifetimeManager())
                .RegisterType<KrUpdateParentTaskExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrInfoForInitiatorGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrStagePermissionsNewGetExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrStartProcessSignalInterceptorStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrValidateStageFieldsStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrTileNewGetExtension>(new ContainerControlledLifetimeManager())

                .RegisterType<KrLaunchProcessCustomExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrLaunchProcessStoreExtension>(new PerResolveLifetimeManager())
                
                .RegisterType<KrTaskKindGetExtension>(new ContainerControlledLifetimeManager())

                ;
        }

        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                // Не важен порядок выполнения
                .RegisterExtension<ICardStoreExtension, KrValidateStageFieldsStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithSingleton())

                // Должно выполнятеся до KrCardStoreExtension
                // Должно выполнятся до KrRecalcStagesStoreExtension
                .RegisterExtension<ICardStoreExtension, KrCheckStageRowModifiedStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 0)
                    .WithUnity(this.UnityContainer))


                .RegisterExtension<ICardNewExtension, KrCardNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyNewMethod())
                .RegisterExtension<ICardGetExtension, KrCardGetExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 2)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreExtension, KrCardStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, KrUpdateParentTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithSingleton())


                .RegisterExtension<ICardGetExtension, KrInfoForInitiatorGetExtension>(x => x
                    // Должно выполнятся после KrCardGetExtension
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer))

                // Должно выполнятся до KrProcessWorkflowStoreExtension
                // Должно выполняться после KrCardStoreExtension
                .RegisterExtension<ICardStoreExtension, KrLaunchProcessStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer))
                // Порядок выполнения неважен, просто лежит рядом с аналогичным
                .RegisterExtension<ICardRequestExtension, KrLaunchProcessCustomExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(KrConstants.LaunchProcessRequestType))

                // Должно выполнятеся после KrLaunchProcessStoreExtension
                .RegisterExtension<ICardStoreExtension, KrStartProcessSignalInterceptorStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithSingleton())

                // Расширение KrProcessWorkflowStoreExtension должно явно выполняться после WfWorkflowStoreExtension с указанным Order 1
                // Должно выполнятся после KrProcessWorkflowStoreExtension
                .RegisterExtension<ICardStoreExtension, KrProcessWorkflowStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardNewExtension, KrLocalTilesNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 6)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardNewMethod.Default, CardNewMethod.Template))
                .RegisterExtension<ICardGetExtension, KrLocalTilesNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 6)
                    .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardNewExtension, KrStagePermissionsNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, KrStagePermissionsNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(this.UnityContainer))

                // Должно выполняться после всех расширений, модифицирующих карточку
                .RegisterExtension<ICardNewExtension, KrTileNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 15)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyNewMethod())
                .RegisterExtension<ICardGetExtension, KrTileNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 15)
                    .WithUnity(this.UnityContainer))

                // Должно выполнятеся после всех расширений в KrProcess и KrCompilers
                .RegisterExtension<ICardStoreExtension, KrCheckGroupBoundariesStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 16)
                    .WithUnity(this.UnityContainer))
                
                // Должно выполнятеся до платформенных, но после типовых
                .RegisterExtension<ICardGetExtension, KrTaskKindGetExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, int.MaxValue)
                    .WithSingleton())
                ;
        }
    }
}