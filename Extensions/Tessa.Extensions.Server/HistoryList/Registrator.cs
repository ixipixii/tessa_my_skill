using Tessa.Cards.Extensions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Server.HistoryList
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {

            this.UnityContainer
                .RegisterType<PnrAddHistoryListGetExtension>(new ContainerControlledLifetimeManager())
                ;
        }

        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {

            extensionContainer
                .RegisterExtension<ICardGetExtension, PnrAddHistoryListGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 101)
                    .WithUnity(this.UnityContainer))
                ;
            ;
        }
    }
}