using Tessa.Extensions.Default.Shared.Workflow.WorkflowEngine;
using Tessa.UI.Workflow;
using Tessa.UI.WorkflowViewer.Actions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Client.Workflow.WorkflowEngine
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            UnityContainer
                .RegisterType<IWorkflowActionUIHandler, KrChangeStateActionUIHandler>(nameof(KrChangeStateActionUIHandler), new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowActionUIHandler, WorkflowCreateCardActionUIHandler>(nameof(WorkflowCreateCardActionUIHandler), new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowActionUIHandler, KrAcquaintanceActionUIHandler>(nameof(KrAcquaintanceActionUIHandler), new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowEngineTileManagerUIExtension, KrCheckStateTileManagerUIExtension>(nameof(KrCheckStateTileManagerUIExtension), new ContainerControlledLifetimeManager())
                .RegisterInstance<IWorkflowActionUIHandler>(
                    nameof(KrDescriptors.RegistrationDescriptor),
                    new WorkflowActionUIHandlerBase(KrDescriptors.RegistrationDescriptor),
                    new ContainerControlledLifetimeManager())
                .RegisterInstance<IWorkflowActionUIHandler>(
                    nameof(KrDescriptors.DeregistrationDescriptor),
                    new WorkflowActionUIHandlerBase(KrDescriptors.DeregistrationDescriptor),
                    new ContainerControlledLifetimeManager())
                ;
        }
    }
}
