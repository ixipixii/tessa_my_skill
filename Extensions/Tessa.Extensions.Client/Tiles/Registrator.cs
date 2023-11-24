using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.UI.Tiles.Extensions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.Tiles
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ITileGlobalExtension, PnrHierarchyContractMenuTilesExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ITileGlobalExtension, PnrCreatePartnerRequestTileExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer))
                //.RegisterExtension<ITileGlobalExtension, PnrRegulationSetCanceledTileExtension>(x => x
                //    .WithOrder(ExtensionStage.AfterPlatform)
                //    .WithUnity(this.UnityContainer))
                ;

            extensionContainer
                .RegisterExtension<ITileLocalExtension, HideDefaultDocumentsTileExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrHierarchyContractMenuTilesExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<PnrCreatePartnerRequestTileExtension>(new ContainerControlledLifetimeManager())
                //.RegisterType<PnrRegulationSetCanceledTileExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<HideDefaultDocumentsTileExtension>(new ContainerControlledLifetimeManager())

                ;
        }
    }
}
