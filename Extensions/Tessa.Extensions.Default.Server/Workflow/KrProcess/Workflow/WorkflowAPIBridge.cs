using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class WorkflowAPIBridge : IWorkflowAPIBridge
    {
        #region fields

        private readonly KrProcessWorkflowManager manager;
        private readonly IWorkflowProcessInfo processInfo;

        #endregion

        #region constructor

        public WorkflowAPIBridge(
            KrProcessWorkflowManager manager,
            IWorkflowProcessInfo processInfo)
        {
            this.manager = manager;
            this.processInfo = processInfo;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IWorkflowTaskInfo SendTask(
            Guid taskTypeID,
            string digest,
            Guid roleID,
            string roleName,
            Dictionary<string, object> taskParameters = null,
            Guid? taskRowID = null,
            Action<CardTask> modifyTaskAction = null)
        {
            CardTask task = this.AddNewTask(taskTypeID, roleID, roleName, taskRowID: taskRowID);
            if (task == null)
            {
                return null;
            }

            task.Digest = digest ?? string.Empty;
            modifyTaskAction?.Invoke(task);
            return this.manager.AddTaskAsync(task, this.processInfo, taskParameters).GetAwaiter().GetResult(); // TODO async

        }

        /// <inheritdoc />
        public void AddActiveTask(Guid taskID)
        {
            var activeTasksSection = this.manager
                .WorkflowContext
                .KrScope
                .GetKrSatellite(this.manager.WorkflowContext.CardID)
                .GetActiveTasksSection();
            if (activeTasksSection.Rows.Any(p => taskID.Equals(p.Fields[KrConstants.KrActiveTasks.TaskID])))
            {
                throw new InvalidOperationException($"Task with id {taskID} is already active.");
            }
            var row = activeTasksSection.Rows.Add();
            row.State = CardRowState.Inserted;
            row.RowID = Guid.NewGuid();
            row.Fields[KrConstants.KrActiveTasks.TaskID] = taskID;
        }

        /// <inheritdoc />
        public bool TryRemoveActiveTask(
            Guid taskID)
        {
            return this.RemoveActiveTaskInternal(taskID, false);
        }

        /// <inheritdoc />
        public void RemoveActiveTask(Guid taskID)
        {
            this.RemoveActiveTaskInternal(taskID, true);
        }

        /// <inheritdoc />
        public IReadOnlyList<Guid> GetActiveTasks() =>
            this.manager.WorkflowContext.KrScope.GetKrSatellite(this.manager.WorkflowContext.CardID)
                .GetActiveTasksSection()
                .Rows
                .Select(p => (Guid)p.Fields[KrConstants.KrActiveTasks.TaskID])
                .ToList()
                .AsReadOnly();

        /// <inheritdoc />
        public void InitCounter(int counterNumber, int initialValue) =>
            this.manager.InitCounterAsync(counterNumber, this.processInfo, initialValue).GetAwaiter().GetResult(); // TODO async

        /// <inheritdoc />
        public WorkflowCounterState DecrementCounter(int counterNumber, int decrementValue = 1) =>
            this.manager.DecrementCounterAsync(counterNumber, this.processInfo, decrementValue).GetAwaiter().GetResult(); // TODO async

        /// <inheritdoc />
        public bool RemoveCounter(int counterNumber) =>
            this.manager.RemoveCounterAsync(counterNumber, this.processInfo).GetAwaiter().GetResult(); // TODO async

        /// <inheritdoc />
        public IList<IWorkflowProcessInfo> ProcessesAwaitingRemoval => this.manager.ProcessesAwaitingRemoval;

        /// <inheritdoc />
        public CardStoreRequest NextRequest => this.manager.NextRequest;

        /// <inheritdoc />
        public bool NextRequestPending => this.manager.NextRequestPending;

        /// <inheritdoc />
        public void NotifyNextRequestPending() => this.manager.NotifyNextRequestPending();

        #endregion

        #region private

        /// <summary>
        /// Создаёт и добавляет задание в запрос на дополнительное сохранение карточки.
        /// </summary>
        /// <param name="taskTypeID">Идентификатор типа создаваемого задания.</param>
        /// <param name="roleID">Идентификатор роли, на которую назначается задание.</param>
        /// <param name="roleName">
        /// Имя роли, на которую назначается задание,
        /// или <c>null</c>, если имя роли определяется автоматически в момент сохранения.
        /// </param>
        /// <param name="planned">
        /// Запланированная дата завершения задания
        /// или <c>null</c>, если планируется дата на 3 дня позже момента создания задания.
        /// </param>
        /// <param name="taskRowID">
        /// Идентификатор отправляемого задания или <c>null</c>, если для задания создаётся новый идентификатор.
        /// </param>
        /// <returns>
        /// Добавленное задание или <c>null</c>, если задание не удалось добавить.
        /// </returns>
        private CardTask AddNewTask(
            Guid taskTypeID,
            Guid roleID,
            string roleName = null,
            DateTime? planned = null,
            Guid? taskRowID = null)
        {
            var newRequest = new CardNewRequest { CardTypeID = taskTypeID };
            CardNewResponse newResponse = this.manager.WorkflowContext.CardRepositoryToCreateTasks.NewAsync(newRequest).GetAwaiter().GetResult(); // TODO async

            var newResponseResult = newResponse.ValidationResult.Build();
            this.manager.ValidationResult.Add(newResponseResult);

            if (!newResponseResult.IsSuccessful)
            {
                return null;
            }

            Card taskCard = newResponse.Card;
            taskCard.ID = taskRowID ?? Guid.NewGuid();

            CardTask task = this.manager.AddTask();
            task.SetCard(taskCard);
            task.SectionRows = newResponse.SectionRows;
            task.RoleID = roleID;
            task.RoleName = roleName;
            task.Planned = planned ?? this.manager.StoreDateTime.AddDays(3);
            task.Settings = new Dictionary<string, object>();

            return task;
        }

        private bool RemoveActiveTaskInternal(
            Guid taskID,
            bool throwOnFailure)
        {
            var activeTasksSection = this.manager
                .WorkflowContext
                .KrScope
                .GetKrSatellite(this.manager.WorkflowContext.CardID)
                .GetActiveTasksSection();
            var activeTaskRow = activeTasksSection.Rows.FirstOrDefault(p => taskID.Equals(p.Fields[KrConstants.KrActiveTasks.TaskID]));
            if (activeTaskRow == null)
            {
                return RemoveActiveTaskFailure(taskID, throwOnFailure);
            }
            switch (activeTaskRow.State)
            {
                case CardRowState.Deleted:
                    return RemoveActiveTaskFailure(taskID, throwOnFailure);
                case CardRowState.Inserted:
                    activeTasksSection.Rows.Remove(activeTaskRow);
                    break;
                case CardRowState.Modified:
                case CardRowState.None:
                    activeTaskRow.State = CardRowState.Deleted;
                    break;
            }

            return true;
        }

        private static bool RemoveActiveTaskFailure(
            Guid taskID,
            bool throwOnFailure)
        {
            return throwOnFailure
                ? throw new InvalidOperationException($"Task with id {taskID} is inactive.")
                : false;
        }

        #endregion
    }
}