using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    /// <summary>
    /// Специальные методы-расширения,
    /// использовние которых обусловлено только в обработчиках этапов.
    /// </summary>
    public static class HandlerExtensions
    {
        /// <summary>
        /// Добавить в историю процесса запись.
        /// </summary>
        /// <param name="satellite">Контекстуальный сателлит</param>
        /// <param name="taskID">ID задания.</param>
        /// <param name="cycle">Текущий цикл.</param>
        /// <param name="advisory">Добавляется запись о рекомендательном согласовании</param>
        public static void AddToHistory(
            this Card satellite,
            Guid? taskID, 
            int cycle = 0,
            bool advisory = false)
        {
            KrErrorHelper.AssertKrSatellte(satellite);

            var historySection = satellite.GetKrApprovalHistorySection();
            var row = historySection.Rows.Add();
            row.State = CardRowState.Inserted;
            row.RowID = Guid.NewGuid();
            row.Fields[KrConstants.Keys.Cycle] = cycle;
            row.Fields[KrConstants.KrApprovalHistory.HistoryRecord] = taskID;
            row.Fields[KrConstants.KrApprovalHistory.Advisory] = advisory;
        }

        /// <summary>
        /// Выполнить действие для каждого этапа в указанной группе.
        /// </summary>
        /// <param name="stages">Все этапы маршрута</param>
        /// <param name="groupID">Группа этапов</param>
        /// <param name="action">Действие над каждо этапом</param>
        public static void ForEachStageInGroup(
            this IList<Stage> stages,
            Guid groupID,
            Action<Stage> action)
        {
            var i = stages.IndexOf(p => p.StageGroupID == groupID);
            var cnt = stages.Count;
            Stage currStage;
            while (i < cnt
                && (currStage = stages[i++]).StageGroupID == groupID)
            {
                action(currStage);
            }
        }

        /// <summary>
        /// Выполнить действие для каждого этапа в указанной группе.
        /// </summary>
        /// <param name="stages">Все этапы маршрута</param>
        /// <param name="groupID">Группа этапов</param>
        /// <param name="action">
        /// Действие над каждо этапом.
        /// Возвращает <c>true</c>, если необходимо продолжить обработку.
        /// Возвращает <c>fale</c>, если необходимо прервать обработку (loop break).
        /// </param>
        public static void ForEachStageInGroup(
            this IList<Stage> stages,
            Guid groupID,
            Func<Stage, bool> action)
        {
            var i = stages.IndexOf(p => p.StageGroupID == groupID);
            var cnt = stages.Count;
            Stage currStage;
            while (i < cnt
                && (currStage = stages[i++]).StageGroupID == groupID)
            {
                if (!action(currStage))
                {
                    break;
                }
            }
        }
    }
}