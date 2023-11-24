using Tessa.Workflow;
using Tessa.Workflow.Actions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Workflow.WorkflowEngine
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<IWorkflowAction, KrChangeStateAction>(
                    nameof(KrChangeStateAction), 
                    new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowAction, WorkflowCreateCardAction>(
                    nameof(WorkflowCreateCardAction),
                    new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowAction, KrAcquaintanceAction>(
                    nameof(KrAcquaintanceAction),
                    new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowAction, KrRegistrationAction>(
                    nameof(KrRegistrationAction),
                    new ContainerControlledLifetimeManager())
                .RegisterType<IWorkflowAction, KrDeregistrationAction>(
                    nameof(KrDeregistrationAction),
                    new ContainerControlledLifetimeManager())


                .RegisterType<IWorkflowEngineTileManagerExtension, KrCheckStateTileManagerExtension>(
                    nameof(KrCheckStateTileManagerExtension),
                    new ContainerControlledLifetimeManager())
                ;
        }

    }
}
