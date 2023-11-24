using Tessa.Cards.Extensions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Server.Files
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                 .RegisterType<PnrHistoryGetFileContentExtension>(new ContainerControlledLifetimeManager())
                 .RegisterType<PnrHistoryGetFileContentExtensionShort>(new ContainerControlledLifetimeManager())
                ;
        }
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardGetFileContentExtension, PnrHistoryGetFileContentExtension>(x => x
                .WithOrder(ExtensionStage.AfterPlatform, -1)
                .WithUnity(this.UnityContainer))

                .RegisterExtension<ICardGetFileContentExtension, PnrHistoryGetFileContentExtensionShort>(x => x
                .WithOrder(ExtensionStage.AfterPlatform, -1)
                .WithUnity(this.UnityContainer));
            ;
        }
    }
}