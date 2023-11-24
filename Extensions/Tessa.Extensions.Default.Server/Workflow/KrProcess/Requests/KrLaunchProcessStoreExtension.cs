using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrLaunchProcessStoreExtension : CardStoreExtension
    {
        private readonly IKrProcessLauncher processLauncher;

        private readonly IKrProcessCache processCache;

        private IKrProcessLaunchResult result;

        private KrProcessInstance krProcess;

        private bool startingProcessNameSet;

        public KrLaunchProcessStoreExtension(
            IKrProcessLauncher processLauncher,
            IKrProcessCache processCache)
        {
            this.processLauncher = processLauncher;
            this.processCache = processCache;
        }

        public override Task BeforeRequest(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !context.Request.TryGetKrProcessInstance(out this.krProcess))
            {
                this.krProcess = null;
                return Task.CompletedTask;
            }

            if (this.krProcess.CardID != context.Request.Card.ID)
            {
                context.ValidationResult.AddError(this, KrErrorHelper.ProcessStartingForDifferentCardID());
                return Task.CompletedTask;
            }

            context.Request.ForceTransaction = true;
            var process = this.processCache.GetSecondaryProcess(this.krProcess.ProcessID);

            if (process.Async)
            {
                // В качестве аванса BeforeRequest KrProcessWorkflowStoreExtension запланирует запуск
                context.Request.SetStartingProcessName(KrConstants.KrSecondaryProcessName);
                this.startingProcessNameSet = true;
            }

            return Task.CompletedTask;
        }

        public override Task BeforeCommitTransaction(
            ICardStoreExtensionContext context)
        {
            if (this.startingProcessNameSet
                && context.Request.TryGetStartingProcessName() == KrConstants.KrSecondaryProcessName)
            {
                // Неважно, что раньше мы попросили запустить процесс.
                // Перепроверим права и перепроверим флаг уже в расширении на старт процесса.
                context.Request.Info.Remove(CardHelper.SystemKeyPrefix + "startProcess");
            }

            if (context.ValidationResult.IsSuccessful()
                && this.krProcess != null)
            {
                this.result = this.processLauncher.Launch(this.krProcess, context);
                context.ValidationResult.Add(this.result.ValidationResult);
            }

            return Task.CompletedTask;
        }

        public override Task AfterRequest(ICardStoreExtensionContext context)
        {
            if (this.result != null
                && this.result is KrProcessLaunchResult res)
            {
                context.Response.SetKrProcessLaunchResult(res);
            }

            return Task.CompletedTask;
        }
    }
}