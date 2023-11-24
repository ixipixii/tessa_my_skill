using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<IKrProcessUIContainer, KrProcessUIContainer>(new ContainerControlledLifetimeManager())
                .RegisterType<ResolutionStageUIHandler>(new PerResolveLifetimeManager())
                .RegisterType<CreateCardUIHandler>(new ContainerControlledLifetimeManager())
                .RegisterType<ProcessManagementUIHandler>(new PerResolveLifetimeManager())
                .RegisterType<UniversalTaskStageTypeUIHandler>(new ContainerControlledLifetimeManager())
                .RegisterType<AddFromTemplateUIHandler>(new ContainerControlledLifetimeManager())
                .RegisterType<DialogUIHandler>(new ContainerControlledLifetimeManager())
                .RegisterType<ApprovalUIHandler>(new PerResolveLifetimeManager())
                .RegisterType<TabCaptionUIHandler>(new PerResolveLifetimeManager())
                .RegisterType<TypedTaskUIHandler>(new ContainerControlledLifetimeManager())
                ;
        }

        public override void FinalizeRegistration()
        {
            this.UnityContainer
                .Resolve<IKrProcessUIContainer>()
                .RegisterUIHandler<ResolutionStageUIHandler>(StageTypeDescriptors.ResolutionDescriptor)
                .RegisterUIHandler<CreateCardUIHandler>(StageTypeDescriptors.CreateCardDescriptor)
                .RegisterUIHandler<ProcessManagementUIHandler>(StageTypeDescriptors.ProcessManagementDescriptor)
                .RegisterUIHandler<UniversalTaskStageTypeUIHandler>(StageTypeDescriptors.UniversalTaskDescriptor)
                .RegisterUIHandler<AddFromTemplateUIHandler>(StageTypeDescriptors.AddFromTemplateDescriptor)
                .RegisterUIHandler<DialogUIHandler>(StageTypeDescriptors.DialogDescriptor)
                .RegisterUIHandler<ApprovalUIHandler>(StageTypeDescriptors.ApprovalDescriptor)
                .RegisterUIHandler<TabCaptionUIHandler>()
                .RegisterUIHandler<TypedTaskUIHandler>(StageTypeDescriptors.TypedTaskDescriptor)
                ;
        }
    }
}