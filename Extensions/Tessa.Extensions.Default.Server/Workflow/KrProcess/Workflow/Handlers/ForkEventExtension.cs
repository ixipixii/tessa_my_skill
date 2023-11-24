using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public sealed class ForkEventExtension: KrEventExtension
    {
        /// <inheritdoc />
        public override Task HandleEvent(
            IKrEventExtensionContext context)
        {
            if (context.ProcessInfo.ProcessTypeName != KrConstants.KrNestedProcessName
                || context.InitiationCause == KrProcessRunnerInitiationCause.StartProcess)
            {
                // Завершается не вложенный процесс
                // или
                // Если процесс запущен и сразу завершен,
                // то это все происходит прямо внутри HandleStageStart
                return Task.CompletedTask;
            }

            var parameters = context.ProcessInfo?.ProcessParameters;
            if (parameters == null)
            {
                return Task.CompletedTask;
            }

            var parentProcessTypeName = parameters.TryGet<string>(KrConstants.Keys.ParentProcessType);
            var parentProcessID = parameters.TryGet<Guid>(KrConstants.Keys.ParentProcessID);

            if (parentProcessTypeName != null
                && parentProcessID != default(Guid))
            {
                var card = context.MainCardAccessStrategy.GetCard();
                var workflowQueue = card.GetWorkflowQueue();
                workflowQueue.AddSignal(
                    parentProcessTypeName,
                    KrConstants.AsyncForkedProcessCompletedSingal,
                    processID: parentProcessID,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.Keys.ProcessID] = context.ProcessInfo.ProcessID,
                        [KrConstants.Keys.ProcessInfoAtEnd] = context.WorkflowProcess.InfoStorage,
                    });
            }

            return Task.CompletedTask;
        }
    }
}