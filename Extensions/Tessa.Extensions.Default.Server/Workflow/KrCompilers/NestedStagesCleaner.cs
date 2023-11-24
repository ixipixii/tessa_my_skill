using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public static class NestedStagesCleaner
    {
        #region public

        public static void ClearAll(
            Card processHolder)
        {
            var stageRows = processHolder.GetStagesSection().Rows;
            foreach (var row in stageRows)
            {
                if (row.TryGet<Guid?>(KrConstants.KrStages.ParentStageRowID) != null)
                {
                    row.State = CardRowState.Deleted;
                }
            }
        }

        public static void ClearGroup(
            Card processHolder,
            Guid? nestedProcessID,
            IList<Guid> stageGroupIDs)
        {
            ClearWithRoots(processHolder, 
                p => stageGroupIDs.Contains(p.TryGet<Guid>(KrConstants.StageGroupID))
                    && p.TryGet<Guid?>(KrConstants.KrStages.NestedProcessID) == nestedProcessID);
        }

        public static void ClearStage(
            Card processHolder,
            Guid? stageRowID)
        {
            ClearWithRoots(processHolder, p => p.RowID == stageRowID);
        }

        #endregion

        #region private

        private static void ClearWithRoots(Card processHolder, Func<CardRow, bool> rootsSelector)
        {
            var stageRows = processHolder.GetStagesSection().Rows;
            var parentQueue = new Queue<Guid>(
                stageRows
                    .Where(rootsSelector)
                    .Select(p => p.RowID));
            
            while (parentQueue.Count != 0)
            {
                var parentStageRowID = parentQueue.Dequeue();
                foreach (var row in stageRows
                    .Where(p => p.TryGet<Guid?>(KrConstants.KrStages.ParentStageRowID) == parentStageRowID))
                {
                    row.State = CardRowState.Deleted;
                    parentQueue.Enqueue(row.RowID);
                }
            }
        }

        #endregion
    }
}