using System;
using System.Collections.Generic;
using System.Text;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Server.Integration.Extensions
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer

                // New

                .RegisterExtension<ICardNewExtension, ClearMDMSentDateNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithSingleton()
                    .WhenMethod(CardNewMethod.Default, CardNewMethod.Template)
                    // заявка на КА, Договор, ДС
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID, PnrCardTypes.PnrContractTypeID, PnrCardTypes.PnrSupplementaryAgreementTypeID))

                // Store

                .RegisterExtension<ICardStoreExtension, PnrSendMessageToMdmCardStoreExtension>(x => x
                    // порядок поставим как можно поздний, чтобы исключить определенные ошибки при сохранении
                    .WithOrder(ExtensionStage.AfterPlatform, int.MaxValue)
                    .WithUnity(this.UnityContainer)
                    // заявка на КА, Договор, ДС
                    .WhenCardTypes(PnrCardTypes.PnrPartnerRequestTypeID, PnrCardTypes.PnrContractTypeID, PnrCardTypes.PnrSupplementaryAgreementTypeID))
                ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrSendMessageToMdmCardStoreExtension>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction),
                        typeof(ICardCache)))          
                ;
        }
    }

}
