using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrStartProcessSignalInterceptorStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            Card card;
            WorkflowQueue queue;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null
                || (queue = card.TryGetWorkflowQueue()) == null
                || queue.IsEmpty)
            {
                return;
            }

            foreach (var item in queue.Items)
            {
                var signalName = item.TryGetSignal()?.Name;
                if (signalName == KrConstants.KrStartProcessSignal)
                {
                    item.Handled = true;
                    StartProcess(context);
                    break;
                }
                if (signalName == KrConstants.KrStartProcessUnlessStartedGlobalSignal)
                {
                    item.Handled = true;
                    if (!await ProcessStartedAsync(context.DbScope, card.ID, context.CancellationToken))
                    {
                        StartProcess(context);
                    }
                    break;
                }
            }
        }

        private static void StartProcess(
            ICardStoreExtensionContext context)
        {
            context.Request.SetStartingProcessName(KrConstants.KrProcessName);
            context.Request.RemoveSecondaryProcess();
        }

        private static async Task<bool> ProcessStartedAsync(
            IDbScope dbScope,
            Guid cardID,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(dbScope.BuilderFactory
                        .Select()
                        .V(true)
                        .From("WorkflowProcesses", "wp").NoLock()
                        .InnerJoin("KrApprovalCommonInfo", "aci").NoLock().On().C("wp", "ID").Equals().C("aci", "ID")
                        .Where().C("aci", "MainCardID").Equals().P("cardID")
                        .Build(),
                        db.Parameter("cardID", cardID))
                    .LogCommand()
                    .ExecuteAsync<bool>(cancellationToken);
            }
        }
    }
}