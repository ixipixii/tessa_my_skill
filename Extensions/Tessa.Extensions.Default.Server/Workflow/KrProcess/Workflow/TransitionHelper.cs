using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public static class TransitionHelper
    {
        public const int NotFound = -1;

        #region public

        public static int TransitByPredicate(
            ICollection<Stage> stages,
            Func<Stage, bool> predicate)
        {
            var transitionIndex = 0;
            foreach (var stage in stages)
            {
                if (predicate(stage))
                {
                    break;
                }

                transitionIndex++;
            }

            return TransitionIndex(stages, transitionIndex);
        }

        public static int TransitToStage(
            ICollection<Stage> stages,
            Guid stageRowID) => TransitByPredicate(stages, s => s.ID == stageRowID);

        public static int TransitToStageGroup(
            ICollection<Stage> stages,
            Guid stageGroupID) => TransitByPredicate(stages, s => s.StageGroupID == stageGroupID);

        public static int TransitToNextGroup(
            ICollection<Stage> stages,
            Guid stageGroupID)
        {
            var transitionIndex = 0;
            var currentGroup = false;
            foreach (var stage in stages)
            {
                if (!currentGroup
                    && stage.StageGroupID == stageGroupID)
                {
                    currentGroup = true;
                }

                if (currentGroup
                    && stage.StageGroupID != stageGroupID)
                {
                    break;
                }

                transitionIndex++;
            }
            return TransitionIndex(stages, transitionIndex); 
        }

        public static int TransitToPreviousGroup(
            ICollection<Stage> stages,
            Guid stageGroupID)
        {
            var transitionIndex = 0;
            var firstStageInGroupIndex = transitionIndex;
            Guid? currentGroupFirstStageID = null;
            Guid? prevStageGroupID = null;
            foreach (var stage in stages)
            {
                if (stage.StageGroupID == stageGroupID)
                {
                    break;
                }

                if (prevStageGroupID != stage.StageGroupID)
                {
                    firstStageInGroupIndex = transitionIndex;
                    currentGroupFirstStageID = stage.RowID;
                    prevStageGroupID = stage.StageGroupID;
                }
                transitionIndex++;
            }

            transitionIndex = currentGroupFirstStageID.HasValue
                ? firstStageInGroupIndex
                : stages.Count;
            return TransitionIndex(stages, transitionIndex);
        }


        /// <summary>
        /// Обновить состояния этапов в маршруте в связи с переходом.
        /// </summary>
        /// <param name="stages"></param>
        /// <param name="startStageRowID"></param>
        /// <param name="finalStateRowID"></param>
        /// <param name="processHolderSatellite"></param>
        public static void ChangeStatesTransition(
            SealableObjectList<Stage> stages,
            Guid startStageRowID,
            Guid finalStateRowID,
            Card processHolderSatellite)
        {
            // Те, что до всего процесса не трогаем
            var transitionIntervalBeginIndex = 0;
            var skipInterval = false;
            var inactiveInterval = false;
            var roots = new Queue<Guid>();
            foreach (var stage in stages)
            {
                if (stage.RowID == finalStateRowID)
                {
                    // Вернулись назад
                    inactiveInterval = true;
                    break;
                }
                if (stage.RowID == startStageRowID)
                {
                    // Перескочили дальше
                    skipInterval = true;
                    break;
                }
                transitionIntervalBeginIndex++;
            }

            if (skipInterval)
            {
                for (var i = transitionIntervalBeginIndex;
                    i < stages.Count && stages[i].RowID != finalStateRowID;
                    i++)
                {
                    var stage = stages[i];
                    // Ставим, что этап пропущен, только если он уже не считается завершенным.
                    if (stage.State != KrStageState.Completed)
                    {
                        stage.State = KrStageState.Skipped;
                        roots.Enqueue(stage.RowID);
                    }
                    if (processHolderSatellite != null)
                    {
                        SetStateToNesteds(processHolderSatellite, roots, KrStageState.Skipped);
                    }
                }
            }
            else if (inactiveInterval)
            {
                var i = transitionIntervalBeginIndex;
                for (;
                    i < stages.Count  && stages[i].RowID != startStageRowID;
                    i++)
                {
                    var stage = stages[i];
                    stage.State = KrStageState.Inactive;
                    roots.Enqueue(stage.RowID);
                }

                if (i < stages.Count)
                {
                    var stage = stages[i];
                    stage.State = KrStageState.Inactive;
                    roots.Enqueue(stage.RowID);
                }
                if (processHolderSatellite != null)
                {
                    SetStateToNesteds(processHolderSatellite, roots, KrStageState.Skipped);
                }
            }
        }

        /// <summary>
        /// Установить состояние "пропущено" для всех последующих этапов.
        /// </summary>
        /// <param name="currentStage">Текущий этап.</param>
        /// <param name="stages">Коллекция этапов.</param>
        /// <param name="processHolderSatellite">Текущий сателлит процесса.</param>
        public static void SetSkipStateToSubsequentStages(
            Stage currentStage,
            SealableObjectList<Stage> stages,
            Card processHolderSatellite)
        {
            if (currentStage.State == KrStageState.Active)
            {
                currentStage.State = KrStageState.Skipped;
            }
            var size = stages.Count;
            var currentStageIndex = stages.IndexOf(currentStage);
            var roots = new Queue<Guid>();
            for (var i = currentStageIndex + 1; i < size; i++)
            {
                var stage = stages[i];
                stage.State = KrStageState.Skipped;
                roots.Enqueue(stage.RowID);
            }
            if (processHolderSatellite != null)
            {
                SetStateToNesteds(processHolderSatellite, roots, KrStageState.Skipped);
            }
        }

        /// <summary>
        /// Установить состояние "не запущен" для всех этапов
        /// </summary>
        /// <param name="stages"></param>
        /// <param name="processHolderSatellite"></param>
        public static void SetInactiveStateToAllStages(
            SealableObjectList<Stage> stages,
            Card processHolderSatellite)
        {
            var roots = new Queue<Guid>();
            foreach (var stage in stages)
            {
                stage.State = KrStageState.Inactive;
                roots.Enqueue(stage.RowID);
            }

            if (processHolderSatellite != null)
            {
                SetStateToNesteds(processHolderSatellite, roots, KrStageState.Inactive);
            }
        }

        #endregion

        #region private

        private static int TransitionIndex(
            ICollection<Stage> stages,
            int transitionIndex)
        {
            return stages.Count == transitionIndex
                ? NotFound
                : transitionIndex;
        }
        
        private static void SetStateToNesteds(
            Card processHolder,
            Queue<Guid> roots,
            KrStageState state)
        {
            var stageRows = processHolder.GetStagesSection().Rows;
            var parentQueue = roots;
            
            while (parentQueue.Count != 0)
            {
                var parentStageRowID = parentQueue.Dequeue();
                foreach (var row in stageRows
                    .Where(p => p.TryGet<Guid?>(KrConstants.KrStages.ParentStageRowID) == parentStageRowID))
                {
                    row.Fields[KrConstants.KrStages.StageStateID] = state.ID;
                    row.Fields[KrConstants.KrStages.StageStateName] = state.TryGetDefaultName();
                    parentQueue.Enqueue(row.RowID);
                }
            }
        }
        
        #endregion
    }
}