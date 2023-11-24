using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.UI.Cards;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.UI
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardUIExtension, PnrIncomingUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrOutgoingUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrPowerAttorneyUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPowerAttorneyTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrRegulationUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrRegulationTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrContractUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrSuppAgrUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrOrderUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOrderTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrServiceNoteUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrServiceNoteTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrPartnerRequestUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrActUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrActTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrUKIncomingUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingUKTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrUKOutgoingUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingUKTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrUKOrderUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOrderUKTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrUKContractUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractUKTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrUKSuppAgrUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementUKTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrPartnerUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.PartnerTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrErrandUIExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.PnrErrandTypeID)
                )
                .RegisterExtension<ICardUIExtension, PnrHideEmptyIncomingReferencesControl>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton())
                ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrIncomingUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrOutgoingUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPowerAttorneyUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrRegulationUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrContractUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrSuppAgrUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrOrderUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrServiceNoteUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerRequestUIExtension>(new ContainerControlledLifetimeManager())
                //.RegisterType<PnrActUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKIncomingUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKOutgoingUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKOrderUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKContractUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKSuppAgrUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerUIExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrErrandUIExtension>(new ContainerControlledLifetimeManager())
                ;
        }
    }
}
