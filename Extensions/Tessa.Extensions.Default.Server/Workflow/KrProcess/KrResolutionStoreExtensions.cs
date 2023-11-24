using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrResolutionStoreExtensions : CardStoreTaskExtension
    {
        private const string KrResolutionRevokingChildren = nameof(KrResolutionRevokingChildren);

        public override async Task StoreTaskBeforeCommitTransaction(ICardStoreTaskExtensionContext context)
        {
            if (!context.IsCompletion
                || context.CompletionOption.ID != DefaultCompletionOptions.Complete
                && context.CompletionOption.ID != DefaultCompletionOptions.Revoke
                || !context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var parentContext = WorkflowScopeContext.Current.StoreContext;
            if (parentContext != null && parentContext.Info.TryGet<bool>(KrResolutionRevokingChildren))
            {
                return;
            }

            if (context.Task.Card
                .Sections[WfHelper.ResolutionSection]
                .Fields.Get<bool>(WfHelper.ResolutionRevokeChildrenField))
            {
                context.StoreContext.Info[KrResolutionRevokingChildren] = BooleanBoxes.True;
            }

            var scope = context.DbScope;
            var db = scope.Db;
            await using var reader = await db
                .SetCommand(
                    scope.BuilderFactory
                        .With("ParentTaskHistory", e => e
                                .Select().C("t", "ParentRowID")
                                .From("TaskHistory", "t").NoLock()
                                .Where().C("t", "TypeID").NotEquals().P("ProjectTypeID")
                                .And().C("t", "RowID").Equals().P("RowID")
                                .UnionAll()
                                .Select().C("t", "ParentRowID")
                                .From("TaskHistory", "t").NoLock()
                                .InnerJoin("ParentTaskHistory", "p")
                                .On().C("p", "RowID").Equals().C("t", "RowID")
                                .Where().C("t", "TypeID").NotEquals().P("ProjectTypeID"),
                            columnNames: new[] { "RowID" },
                            recursive: true)
                        .Select().C("t", "ProcessID", "ProcessKind")
                        .From("TaskHistory", "t").NoLock()
                        .InnerJoin("ParentTaskHistory", "p").NoLock()
                        .On().C("p", "RowID").Equals().C("t", "RowID")
                        .Where().C("t", "TypeID").Equals().P("ProjectTypeID")
                        .And().NotExists(e => e
                            .Select().V(null)
                            .From("TaskHistory", "t").NoLock()
                            .InnerJoin("ParentTaskHistory", "p").NoLock()
                            .On().C("p", "RowID").Equals().C("t", "ParentRowID")
                            .InnerJoin("WfSatelliteTaskHistory", "s").NoLock()
                            .On().C("s", "RowID").Equals().C("t", "RowID")
                            .Where().C("t", "Completed").IsNull()
                            .Or().C("s", "Controlled").Equals().V(false))
                        .Build(),
                    db.Parameter("RowID", context.Task.RowID),
                    db.Parameter("ProjectTypeID", DefaultTaskTypes.WfResolutionProjectTypeID))
                .ExecuteReaderAsync(context.CancellationToken);
            if (await reader.ReadAsync(context.CancellationToken))
            {
                var processID = reader.GetGuid(0);
                var processName = reader.GetString(1);

                if (processName == KrConstants.KrProcessName ||
                    processName == KrConstants.KrSecondaryProcessName ||
                    processName == KrConstants.KrNestedProcessName)
                {
                    context.Request.Card
                        .GetWorkflowQueue()
                        .AddSignal(name: KrConstants.KrPerformSignal, processID: processID, processTypeName: processName);
                }
            }
        }
    }
}
