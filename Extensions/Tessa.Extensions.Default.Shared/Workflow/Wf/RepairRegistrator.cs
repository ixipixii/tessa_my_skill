using Tessa.Cards.Extensions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Shared.Workflow.Wf
{
    [Registrator(Tag = RegistratorTag.DefaultForRepair)]
    public sealed class RepairRegistrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<WfSatelliteRepairExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<WfTaskSatelliteRepairExtension>(new ContainerControlledLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            extensionContainer
                .RegisterExtension<ICardRepairExtension, WfSatelliteRepairExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardRepairExtension, WfTaskSatelliteRepairExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer))
                ;
        }
    }
}
