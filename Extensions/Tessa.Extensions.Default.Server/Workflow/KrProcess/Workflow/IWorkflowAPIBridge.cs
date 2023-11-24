using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Workflow;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IWorkflowAPIBridge
    {
        /// <summary>
        /// Отправить задание.
        /// </summary>
        /// <param name="taskTypeID">Идентификатор типа задания</param>
        /// <param name="digest">Дайджест задания</param>
        /// <param name="roleID">Роль, на которую отправляется задание</param>
        /// <param name="roleName">Имя роли, на которую отправляется задание</param>
        /// <param name="taskParameters">Параметры задания.</param>
        /// <param name="taskRowID">Опицонально идентификатор задания.</param>
        /// <param name="modifyTaskAction">Действие, выполняемое перед отправкой задания</param>
        /// <returns></returns>
        IWorkflowTaskInfo SendTask(
            Guid taskTypeID,
            string digest,
            Guid roleID,
            string roleName = null,
            Dictionary<string, object> taskParameters = null,
            Guid? taskRowID = null,
            Action<CardTask> modifyTaskAction = null);

        /// <summary>
        /// Добавить задание в список активных.
        /// </summary>
        /// <param name="taskID">Идентификатор задания</param>
        void AddActiveTask(Guid taskID);

        /// <summary>
        /// Удалить задание из списка активных.
        /// В случае неудачи будет возвращено false.
        /// </summary>
        /// <param name="taskID">Идентификатор задания</param>
        /// <returns>Активное задание успешно удалено из списка</returns>
        bool TryRemoveActiveTask(
            Guid taskID);

        /// <summary>
        /// Удалить задание из списка активных.
        /// В случае неудачи будет выброшено исключение
        /// </summary>
        /// <param name="taskID">Идентификатор задания</param>
        void RemoveActiveTask(Guid taskID);

        /// <summary>
        /// Получить список идентификаторов активных заданий
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<Guid> GetActiveTasks();

        /// <summary>
        /// Инициализировать счетчик.
        /// </summary>
        /// <param name="counterNumber"></param>
        /// <param name="initialValue"></param>
        void InitCounter(
            int counterNumber,
            int initialValue);

        /// <summary>
        /// Уменьшить значение счетчика.
        /// </summary>
        /// <param name="counterNumber"></param>
        /// <param name="decrementValue"></param>
        /// <returns></returns>
        WorkflowCounterState DecrementCounter(
            int counterNumber,
            int decrementValue = 1);

        /// <summary>
        /// Удалить счетчик.
        /// </summary>
        /// <param name="counterNumber"></param>
        /// <returns></returns>
        bool RemoveCounter(
            int counterNumber);

        /// <summary>
        /// Список процессов, ожидающих завершения.
        /// </summary>
        IList<IWorkflowProcessInfo> ProcessesAwaitingRemoval { get; }

        /// <summary>
        /// Запрос на следующее сохранение.
        /// </summary>
        CardStoreRequest NextRequest { get; }

        /// <summary>
        /// Необходимо ли выполнить следующее сохранение.
        /// </summary>
        bool NextRequestPending { get; }

        /// <summary>
        /// Уведомить WorkflowAPI о необходимости выполнения следующего сохранения.
        /// </summary>
        void NotifyNextRequestPending();
    }
}