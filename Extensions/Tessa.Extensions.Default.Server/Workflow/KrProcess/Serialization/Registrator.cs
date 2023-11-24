using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    [Registrator]
    public sealed class Registrator: RegistratorBase
    {
        public override void InitializeExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterKrStageRowExtensionTypes()
                ;
        }


        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<ExtraSourcesStageRowExtension>(new ContainerControlledLifetimeManager());
        }

        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<IKrStageRowExtension, ExtraSourcesStageRowExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenRouteCardTypes(RouteCardType.Template))
                ;
        }
    }
}