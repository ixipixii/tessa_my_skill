using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Notices;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardStoreTaskExtension, PnrChangeTaskResultStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrAddCommentDigestStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrCheckingForAnAttachedFileTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrCheckingPartnerStatusTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrSetPartnerStatusTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrRegistrationDocumentTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrChangeSignDateStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardStoreTaskExtension, PnrChangeRegistrationDateStoreTaskExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithUnity(this.UnityContainer))
            ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrRegistrationDocumentTaskExtension>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction),
                        typeof(IKrTokenProvider),
                        typeof(IKrProcessLauncher),

                        new ResolvedParameter<CardWithoutTransactionStrategy>(CardTransactionStrategyNames.WithoutTransaction),
                        typeof(INotificationManager),
                        typeof(INotificationRoleAggregator)
                        ))
                ;
        }
    }
}
