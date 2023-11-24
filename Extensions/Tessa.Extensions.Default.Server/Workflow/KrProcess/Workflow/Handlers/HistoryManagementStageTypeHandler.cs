using System;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class HistoryManagementStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public HistoryManagementStageTypeHandler(IKrScope krScope)
        {
            this.KrScope = krScope;
        }

        #endregion

        #region Protected Properties

        protected IKrScope KrScope { get; set; }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            if (!context.MainCardID.HasValue)
            {
                context.ValidationResult.AddError(this, "$KrStages_HistoryManagement_GlobalContextError");
                return StageHandlerResult.SkipResult;
            }

            var resolver = context.TaskHistoryResolver;
            var taskHistoryGroupID =
                context.Stage.SettingsStorage.TryGet<Guid?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.TaskHistoryGroupTypeID);
            var parentTaskHistoryGroupID =
                context.Stage.SettingsStorage.TryGet<Guid?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.ParentTaskHistoryGroupTypeID);
            var newIteration =
                context.Stage.SettingsStorage.TryGet<bool?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.NewIteration);
            if (taskHistoryGroupID.HasValue)
            {
                var group = resolver.ResolveTaskHistoryGroup(taskHistoryGroupID.Value, parentTaskHistoryGroupID, newIteration == true);
                this.KrScope.SetCurrentHistoryGroup(context.MainCardID.Value, group.RowID);
            }
            else
            {
                this.KrScope.SetCurrentHistoryGroup(context.MainCardID.Value, null);
            }

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}