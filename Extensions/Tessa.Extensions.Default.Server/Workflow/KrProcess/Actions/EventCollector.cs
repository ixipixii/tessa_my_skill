using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Actions
{
    public sealed class EventCollector: KrEventExtension
    {
        private readonly IKrProcessLauncher processLauncher;

        private readonly IKrProcessCache processCache;

        public EventCollector(
            IKrProcessLauncher processLauncher,
            IKrProcessCache processCache)
        {
            this.processLauncher = processLauncher;
            this.processCache = processCache;
        }

        public override Task HandleEvent(
            IKrEventExtensionContext context)
        {
            var actions = this.processCache.GetActionsByType(context.EventType);

            if (actions.Count == 0)
            {
                return Task.CompletedTask;
            }

            var pb = KrProcessBuilder.CreateProcess();
            if (context.MainCardID.HasValue)
            {
                pb.SetCard(context.MainCardID.Value);
            }

            if (context.Info is Dictionary<string, object> info)
            {
                pb.SetProcessInfo(info);
            }

            var baseProcess = pb.Build();
            var specifiedParameters = new KrProcessServerLauncher.SpecificParameters();
            specifiedParameters.MainCardAccessStrategy = context.MainCardAccessStrategy;
            specifiedParameters.RaiseErrorWhenExecutionIsForbidden = false;
            foreach (var action in actions)
            {
                var process = KrProcessBuilder
                    .ModifyProcess(baseProcess)
                    .SetProcess(action.ID)
                    .Build();

                var result = this.processLauncher.Launch(process, context.CardExtensionContext, specifiedParameters);
                context.ValidationResult.Add(result.ValidationResult);
            }

            return Task.CompletedTask;
        }
    }
}