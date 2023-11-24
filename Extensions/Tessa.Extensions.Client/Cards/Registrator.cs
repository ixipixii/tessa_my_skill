using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.Cards
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            UnityContainer
                .RegisterType<PnrIncomingStoreExtension>(new ContainerControlledLifetimeManager())
                ;
        }

        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardStoreExtension, PnrIncomingStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(UnityContainer)
                    .WhenCardTypes(PnrCardTypes.PnrIncomingTypeID))
                ;
        }
    }
}