using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.UI.Cards;
using Tessa.UI.Files;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.Files
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<IFileExtension, DocxToPdfFileExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer))
                ;
        }

        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<DocxToPdfFileExtension>(new ContainerControlledLifetimeManager())
                ;
        }
    }
}