using System;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Collections;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class PartialGroupRecalcStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public PartialGroupRecalcStageTypeHandler(
            [Dependency(KrExecutorNames.CacheExecutor)] Func<IKrExecutor> executorFunc)
        {
            this.ExecutorFunc = executorFunc;
        }

        #endregion

        #region Protected Properties

        protected Func<IKrExecutor> ExecutorFunc { get; set; }

        #endregion

        #region Protected Methods

        protected virtual bool FindHeads(
            WorkflowProcess modifiedProcess,
            WorkflowProcess originalProcess,
            Guid currentStageID,
            Guid currentGroupID,
            out int originalIdx,
            out int modifiedIdx)
        {
            originalIdx = 0;
            modifiedIdx = 0;
            while (originalIdx < originalProcess.Stages.Count
                && originalProcess.Stages[originalIdx].StageGroupID != currentGroupID)
            {
                originalIdx++;
            }

            while (originalIdx < originalProcess.Stages.Count
                && modifiedIdx < modifiedProcess.Stages.Count
                && originalProcess.Stages[originalIdx] == modifiedProcess.Stages[modifiedIdx])
            {
                if (originalProcess.Stages[originalIdx].ID == currentStageID)
                {
                    return true;
                }

                originalIdx++;
                modifiedIdx++;
            }

            return false;
        }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var baseResult = base.HandleStageStart(context);
            if (baseResult != StageHandlerResult.EmptyResult)
            {
                return baseResult;
            }

            if (!context.MainCardID.HasValue)
            {
                return StageHandlerResult.SkipResult;
            }

            var workflowProcessCopy = context.WorkflowProcess
                .CloneWithDedicatedStageGroup(context.Stage.StageGroupID, true);
            var executor = this.ExecutorFunc();
            var ctx = new KrExecutionContext(
                context.CardExtensionContext,
                context.MainCardAccessStrategy,
                context.MainCardID,
                context.MainCardTypeID,
                context.MainCardTypeName,
                context.MainCardTypeCaption,
                context.MainCardDocTypeID,
                context.KrComponents,
                workflowProcessCopy,
                compilationResult: null,
                executionUnits: new[] { context.Stage.StageGroupID });
            var result = executor.Execute(ctx);
            context.ValidationResult.Add(result.Result);

            var stageGroupID = context.Stage.StageGroupID;
            if (!this.FindHeads(
                workflowProcessCopy,
                context.WorkflowProcess,
                context.Stage.ID,
                stageGroupID,
                out var originalIdx,
                out var modifiedIdx))
            {
                return StageHandlerResult.SkipResult;
            }

            originalIdx++;
            modifiedIdx++;
            var newStages = workflowProcessCopy.Stages;
            var stages = context.WorkflowProcess.Stages;
            while (modifiedIdx < newStages.Count
                && originalIdx < stages.Count
                && stages[originalIdx].StageGroupID == stageGroupID)
            {
                stages[originalIdx] = newStages[modifiedIdx];
                originalIdx++;
                modifiedIdx++;
            }

            // Если после пересчета этапов больше, чем было
            if (modifiedIdx < newStages.Count)
            {
                // В конце маршрута
                if (originalIdx == stages.Count)
                {
                    stages.AddRange(newStages.Skip(modifiedIdx));
                }
                // Еще есть группы после текущей
                else
                {
                    stages.InsertRange(originalIdx, newStages.Skip(modifiedIdx).ToList().AsReadOnly());
                }
            }
            else
            {
                // Если после пересчета в текущей группе оказалось меньше этапов, чем было
                // надо аккуратно линее поудалять.
                while (originalIdx < stages.Count
                    && stages[originalIdx].StageGroupID == stageGroupID)
                {
                    context.WorkflowProcess.Stages.RemoveAt(originalIdx);
                    originalIdx++;
                }
            }

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}