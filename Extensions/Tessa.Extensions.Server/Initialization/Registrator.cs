using System;
using System.Collections.Generic;
using System.Text;
using Tessa.Extensions;
using Tessa.Platform.Initialization;
using Tessa.Platform.Runtime;
using Unity.Lifetime;
using Unity;
using Tessa.Extensions.Server.Initialization;

namespace Tessa.FdProcesses.Server.Fd.Initialization
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrServerInitializationExtension>(new ContainerControlledLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<IServerInitializationExtension, PnrServerInitializationExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenApplications(ApplicationIdentifiers.WebClient))
                ;
        }
    }
}
