using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrLaunchProcessCustomExtension : CardRequestExtension
    {
        private readonly IKrProcessLauncher processLauncher;

        public KrLaunchProcessCustomExtension(
            IKrProcessLauncher processLauncher)
        {
            this.processLauncher = processLauncher;
        }

        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !context.Request.TryGetKrProcessInstance(out var processInstance))
            {
                return;
            }

            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                using var transact = await db.BeginTransactionAsync(context.CancellationToken);
                var result = this.processLauncher.Launch(processInstance, context);
                if (result is KrProcessLaunchResult typedResult)
                {
                    context.Response.SetKrProcessLaunchResult(typedResult);
                }

                if (result.ValidationResult.IsSuccessful())
                {
                    await transact.CommitAsync(context.CancellationToken);
                }
                else
                {
                    await transact.RollbackAsync(context.CancellationToken);
                }

                context.ValidationResult.Add(result.ValidationResult);
            }
        }

    }
}