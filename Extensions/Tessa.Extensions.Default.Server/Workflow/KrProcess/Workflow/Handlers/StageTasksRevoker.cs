using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public sealed class StageTasksRevoker : IStageTasksRevoker
    {
        private readonly IDbScope dbScope;
        private readonly IKrScope krScope;
        private readonly ICardGetStrategy cardGetStrategy;
        private readonly ISession session;
        private readonly ICardMetadata cardMetadata;

        public StageTasksRevoker(
            IDbScope dbScope,
            IKrScope krScope,
            ICardGetStrategy cardGetStrategy,
            ISession session,
            ICardMetadata cardMetadata)
        {
            this.dbScope = dbScope;
            this.krScope = krScope;
            this.cardGetStrategy = cardGetStrategy;
            this.session = session;
            this.cardMetadata = cardMetadata;
        }

        /// <inheritdoc />
        public bool RevokeAllStageTasks(IStageTasksRevokerContext context)
        {
            if (context.Context.ProcessInfo is null
                || context.Context.MainCardID is null)
            {
                return true;
            }
            context.CardID = context.Context.MainCardID.Value;

            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                // Получение списка заданий из таблицы WorkflowTasks
                var currentTasks = db.SetCommand(
                        this.dbScope.BuilderFactory
                            .Select()
                            .C("RowID")
                            .From("WorkflowTasks").NoLock()
                            .Where().C("ProcessRowID").Equals().P("pid")
                            .Build(),
                        db.Parameter("pid", context.Context.ProcessInfo.ProcessID))
                    .LogCommand()
                    .ExecuteList<Guid>();

                switch (currentTasks.Count)
                {
                    case 0:
                        // Заданий нет, прерывание этапа завершено.
                        return true;
                    case 1:
                        // Задание есть, его нужно отозвать.
                        context.TaskID = currentTasks[0];
                        return this.RevokeTask(context);
                    default:
                        context.TaskIDs = currentTasks;
                        return this.RevokeTasks(context) == 0;
                }
            }
        }

        /// <inheritdoc />
        public bool RevokeTask(IStageTasksRevokerContext context)
        {
            using (this.dbScope.Create())
            {
                var validationResult = context.Context.ValidationResult;
                var card = this.krScope.GetMainCard(context.CardID);
                var cardTasks = card.TryGetTasks();
                if (cardTasks is null
                    || cardTasks.All(p => p.RowID != context.TaskID))
                {
                    var db = this.dbScope.Db;
                    var taskContexts = this.cardGetStrategy.TryLoadTaskInstancesAsync(
                        card.ID,
                        card,
                        db,
                        this.cardMetadata,
                        validationResult,
                        this.session.User.ID,
                        getTaskMode: CardGetTaskMode.All,
                        loadCalendarInfo: false,
                        taskRowIDList: new[] { context.TaskID }).GetAwaiter().GetResult(); // TODO async
                    foreach (var taskContext in taskContexts)
                    {
                        this.cardGetStrategy.LoadSectionsAsync(taskContext).GetAwaiter().GetResult(); // TODO async
                    }
                }

                var task = cardTasks?.FirstOrDefault(p => p.RowID == context.TaskID);
                if (task is null)
                {
                    return true;
                }

                RevokeTaskInternal(context, task);

                return false;
            }
        }

        /// <inheritdoc />
        public int RevokeTasks(IStageTasksRevokerContext context)
        {
            var tasksToRevoke = context.TaskIDs;
            if (tasksToRevoke.Count == 0)
            {
                return 0;
            }

            var tasksToLoad = tasksToRevoke;
            var card = this.krScope.GetMainCard(context.CardID);
            var cardTasks = card.TryGetTasks();
            if (cardTasks != null)
            {
                tasksToLoad = new List<Guid>(tasksToLoad);

                foreach (var cardTask in cardTasks)
                {
                    tasksToLoad.Remove(cardTask.RowID);
                }
            }

            if (tasksToLoad.Count > 0)
            {
                IList<CardGetContext> taskContexts;
                using (this.dbScope.Create())
                {
                    taskContexts = this.cardGetStrategy.TryLoadTaskInstancesAsync(
                        card.ID,
                        card,
                        this.dbScope.Db,
                        this.cardMetadata,
                        context.Context.ValidationResult,
                        this.session.User.ID,
                        getTaskMode: CardGetTaskMode.All, loadCalendarInfo: false, taskRowIDList: tasksToLoad).GetAwaiter().GetResult(); // TODO async
                }
                foreach (var taskContext in taskContexts)
                {
                    this.cardGetStrategy.LoadSectionsAsync(taskContext).GetAwaiter().GetResult(); // TODO async
                }
            }

            cardTasks = card.TryGetTasks();
            if (cardTasks == null)
            {
                return 0;
            }

            var tasksRevoked = 0;
            foreach (var taskToRevoke in cardTasks)
            {
                if (tasksToRevoke.Contains(taskToRevoke.RowID))
                {
                    RevokeTaskInternal(context, taskToRevoke);
                    tasksRevoked++;
                }
            }

            return tasksRevoked;
        }

        private static void RevokeTaskInternal(
            IStageTasksRevokerContext context,
            CardTask task)
        {
            task.Action = CardTaskAction.Complete;
            task.State = CardRowState.Deleted;
            task.Flags = task.Flags & ~CardTaskFlags.Locked | CardTaskFlags.UnlockedForAuthor | CardTaskFlags.HistoryItemCreated;
            task.OptionID = context.OptionID ?? DefaultCompletionOptions.Cancel;

            if (context.RemoveFromActive)
            {
                context.Context.WorkflowAPI.TryRemoveActiveTask(task.RowID);
            }

            context.TaskModificationAction?.Invoke(task);
        }
    }
}