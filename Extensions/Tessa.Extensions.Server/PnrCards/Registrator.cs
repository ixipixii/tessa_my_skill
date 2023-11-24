using System;
using System.Collections.Generic;
using System.Text;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Data;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Server.PnrCards
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                // Фикс RowID версий файлов в договорах с покупателями для поддержки работы ссылок в CRM.
                .RegisterExtension<ICardStoreExtension, CrmFixFileVersionCardStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractTypeID, PnrCardTypes.PnrSupplementaryAgreementTypeID)
                    )
                .RegisterExtension<ICardRequestExtension, UserDepartmentInfoExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.UserDepartmentInfo)
                    )
                .RegisterExtension<ICardRequestExtension, LegalEntityIndexExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.LegalEntityIndex)
                    )
                .RegisterExtension<ICardNewExtension, PnrContractNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractTypeID)
                    )
                // КА - загрузка
                .RegisterExtension<ICardGetExtension, PnrPartnerGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WhenCardTypes(DefaultCardTypes.PartnerTypeID)
                    )
                // КА - сохранение
                .RegisterExtension<ICardStoreExtension, PnrPartnerStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WhenCardTypes(DefaultCardTypes.PartnerTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrOrganizationStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WhenCardTypes(PnrCardTypes.PnrOrganizationTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrOrderStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOrderTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrRegulationStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrRegulationTypeID)
                    )
                .RegisterExtension<ICardGetExtension, CardPermissionsGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyCardType())
                .RegisterExtension<ICardNewExtension, PnrDocTypeNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingTypeID, PnrCardTypes.PnrOutgoingTypeID, PnrCardTypes.PnrContractTypeID, PnrCardTypes.PnrSupplementaryAgreementTypeID,
                    PnrCardTypes.PnrActTypeID, PnrCardTypes.PnrOrderTypeID, PnrCardTypes.PnrServiceNoteTypeID, PnrCardTypes.PnrErrandTypeID, PnrCardTypes.PnrRegulationTypeID,
                    PnrCardTypes.PnrPowerAttorneyTypeID, PnrCardTypes.PnrTenderTypeID, PnrCardTypes.PnrPartnerRequestTypeID, PnrCardTypes.PnrTemplateTypeID,
                    PnrCardTypes.PnrIncomingUKTypeID, PnrCardTypes.PnrOutgoingUKTypeID, PnrCardTypes.PnrOrderUKTypeID, PnrCardTypes.PnrContractUKTypeID, PnrCardTypes.PnrSupplementaryAgreementUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrIncomingStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrOutgoingStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrUKIncomingStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrUKOutgoingStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrUKOrderStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOrderUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrContractStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrSuppAgrStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrSuppAgrNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrTemplateNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrTemplateTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrPowerAttorneyNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPowerAttorneyTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrOutgoingNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrIncomingNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingTypeID)
                    )
                // Заявка на контрагента
                .RegisterExtension<ICardGetExtension, PnrPartnerRequestGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrPartnerRequestStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID)
                    )
                .RegisterExtension<ICardGetExtension, PartnerPermissionsGetExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrPowerAttorneyStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPowerAttorneyTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrPartnerNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrPartnerRequestNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrUKIncomingNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingUKTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrUKOutgoingNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrOutgoingUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrErrandStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrErrandTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrSetPerformersNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrActTypeID, PnrCardTypes.PnrOrderTypeID, PnrCardTypes.PnrPowerAttorneyTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrUKContractNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractUKTypeID)
                    )
                .RegisterExtension<ICardNewExtension, PnrUKSuppAgrNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrUKContractStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrContractUKTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrUKSuppAgrStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementUKTypeID)
                    )
                //.RegisterExtension<ICardGetExtension, PnrSuppAgrGetExtension>(x => x
                //    .WithOrder(ExtensionStage.BeforePlatform)
                //    .WithUnity(this.UnityContainer)
                //    .WhenCardTypes(PnrCardTypes.PnrSupplementaryAgreementTypeID)
                //    )
                .RegisterExtension<ICardStoreExtension, PnrServiceNoteStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrServiceNoteTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrTemplateStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrTemplateTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrActStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrActTypeID)
                    )
                .RegisterExtension<ICardStoreExtension, PnrSetSendToStageNameStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform)
                    .WithUnity(this.UnityContainer)
                    )
                ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<CrmFixFileVersionCardStoreExtension>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<ICardRepository>(CardRepositoryNames.DefaultWithoutTransaction),
                        new ResolvedParameter<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction),
                        typeof(ITransactionStrategy),
                        typeof(ICardTransactionStrategy),
                        typeof(ICardGetStrategy),
                        typeof(ITessaServerSettings)))
                .RegisterType<UserDepartmentInfoExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerRequestGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerRequestStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<LegalEntityIndexExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrContractNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrOrderStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrRegulationStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrDocTypeNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrIncomingStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrOutgoingStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKIncomingStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKOutgoingStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKOrderStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrContractStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrSuppAgrStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrSuppAgrNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrTemplateNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPowerAttorneyNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrOutgoingNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrIncomingNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PartnerPermissionsGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPowerAttorneyStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrPartnerRequestNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKIncomingNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrUKOutgoingNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrErrandStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrSetPerformersNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrServiceNoteStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrTemplateStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrActStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrSetSendToStageNameStoreExtension>(new ContainerControlledLifetimeManager())
                ;
        }
    }
}
