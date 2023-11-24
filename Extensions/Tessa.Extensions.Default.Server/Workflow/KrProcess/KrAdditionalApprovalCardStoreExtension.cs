using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Data;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Roles;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public class KrAdditionalApprovalCardStoreExtension : CardStoreExtension
    {
        #region Private Methods

        private Task InsertChildRecordAsync(
            CardTask task,
            Guid parentID,
            DateTime storeDateTime,
            IQueryExecutor executor,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            // при вставке задания у него присутствуют все секции,
            // а вся его информация актуальна в объекте task

            // метод следует вызывать только в том случае, если задание parentID ещё не завершено

            Card taskCard = task.Card;
            Dictionary<string, object> taskFields = taskCard.Sections["TaskCommonInfo"].RawFields;
            Dictionary<string, object> additioanlApprovalFields = taskCard.Sections[nameof(KrAdditionalApprovalTaskInfo)].RawFields;

            DateTime created = taskCard.Created ?? storeDateTime;
            DateTime planned = task.Planned ?? storeDateTime;
            string parentComment = taskFields.TryGet<string>("Info");
            bool isResponsible = additioanlApprovalFields.TryGet<bool>("IsResponsible");

            return executor
                .ExecuteNonQueryAsync(
                    builderFactory
                        .InsertInto("KrAdditionalApprovalInfo",
                            "ID", "RowID", "PerformerID", "PerformerName", "UserID",
                            "UserName", "OptionID", "OptionCaption", "Comment", "Answer",
                            "Created", "Planned", "InProgress", "Completed", "IsResponsible")
                        .Values(v=>v
                            .P("ParentID", "TaskID", "RoleID", "RoleName").V(null)
                            .V(null).V(null).V(null).P("Comment").V(null)
                            .P("Created", "Planned").V(null).V(null).P("IsResponsible"))
                        .Build(),
                    cancellationToken,
                    executor.Parameter("ParentID", parentID),
                    executor.Parameter("TaskID", task.RowID),
                    executor.Parameter("RoleID", task.RoleID),
                    executor.Parameter("RoleName", SqlHelper.NotNull(task.RoleName, RoleHelper.RoleNameMaxLength)),
                    executor.Parameter("Comment", SqlHelper.Nullable(parentComment)),
                    executor.Parameter("Created", created),
                    executor.Parameter("Planned", planned),
                    executor.Parameter("IsResponsible", isResponsible));
        }

        private static Task ClearMainTaskInfoAsync(
            Guid parentID,
            IQueryExecutor executor,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            return executor
                .ExecuteNonQueryAsync(
                    builderFactory
                        .DeleteFrom("KrAdditionalApprovalUsers")
                        .Where().C("ID").Equals().P("ParentID").Z()

                        .Update("KrAdditionalApproval")
                            .C("FirstIsResponsible").Assign().V(false)
                            .C("Comment").Assign().V(null)
                        .Where().C("ID").Equals().P("ParentID")
                        .Build(),
                    cancellationToken,
                    executor.Parameter("ParentID", parentID));
        }


        private static Task<bool> HasTaskAsync(
            Guid taskRowID,
            DbManager db,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            return db
                .SetCommand(
                    builderFactory
                        .Select().V(true)
                        .From("Tasks").NoLock()
                        .Where().C("RowID").Equals().P("RowID")
                        .Build(),
                    db.Parameter("RowID", taskRowID))
                .LogCommand()
                .ExecuteAsync<bool>(cancellationToken);
        }


        private Task UpdateChildRecordAsync(
            CardTask task,
            IQueryExecutor executor,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            // если даже родительское задание уже завершено, то update не упадёт, а просто не обновит строки
            var builder = builderFactory
                .Update("KrAdditionalApprovalInfo")
                    .C("InProgress").Assign().C("th", "InProgress")
                    .C("PerformerID").Assign().C("th", "RoleID")
                    .C("PerformerName").Assign().C("th", "RoleName")
                    .C("UserID").Assign().C("th", "UserID")
                    .C("UserName").Assign().C("th", "UserName");

            if (task.State == CardRowState.Deleted)
                builder
                    .C("Completed").Assign().C("th", "Completed")
                    .C("OptionID").Assign().C("th", "OptionID")
                    .C("OptionCaption").Assign().C("th", "OptionCaption");

            var parameters = new List<DataParameter>
            {
                executor.Parameter("TaskID", task.RowID),
            };

            if (task.Action == CardTaskAction.Complete && task.OptionID != null)
            {
                string answer = null;

                if (task.OptionID != DefaultCompletionOptions.Revoke)
                {
                    Card taskCard = task.TryGetCard();
                    StringDictionaryStorage<CardSection> taskSections;
                    if (taskCard != null
                        && (taskSections = taskCard.TryGetSections()) != null
                        && taskSections.TryGetValue(nameof(KrAdditionalApprovalTaskInfo), out CardSection additionalApprovalSection))
                    {
                        answer = additionalApprovalSection.RawFields.TryGet<string>("Comment");
                        if (!string.IsNullOrWhiteSpace(answer))
                        {
                            int firstLineEnding = answer.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                            if (firstLineEnding > 0)
                            {
                                answer = answer.Substring(firstLineEnding + Environment.NewLine.Length);
                            }
                        }
                    }
                }
                else
                {
                    // удаляем комментарий автора, если он есть, отделённый переводом строки от прочей информации
                    answer = task.Result;
                    if (answer != null)
                    {
                        int firstLineEnding = answer.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                        if (firstLineEnding > 0)
                        {
                            answer = answer.Substring(firstLineEnding + Environment.NewLine.Length);
                        }
                    }
                }

                if (answer != null)
                {
                    builder
                        .C("Answer").Assign().P("Answer");
                    parameters.Add(
                        executor.Parameter("Answer", SqlHelper.Nullable(answer)));
                }
            }

            builder
                .From("TaskHistory", "th").NoLock()
                .Where().C("KrAdditionalApprovalInfo", "RowID").Equals().C("th", "RowID")
                    .And().C("KrAdditionalApprovalInfo", "RowID").Equals().P("TaskID");

            return executor
                .ExecuteNonQueryAsync(
                    builder.Build(),
                    cancellationToken,
                    parameters.ToArray());
        }

        #endregion

        #region Base Overrides

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            var card = context.Request.TryGetCard();
            if (card == null)
            {
                return;
            }

            var tasks = card.TryGetTasks();
            if (tasks == null || tasks.Count == 0)
            {
                return;
            }

            await using (context.DbScope.Create())
            {
                var executor = context.DbScope.Executor;
                var builderFactory = context.DbScope.BuilderFactory;
                foreach (var task in context.Request.Card.Tasks)
                {
                    var parentRowID = task.ParentRowID;
                    if (parentRowID == null || task.TypeID != DefaultTaskTypes.KrAdditionalApprovalTypeID)
                        continue;

                    if (task.State == CardRowState.Inserted)
                    {
                        // родительское задание расположено в той же карточке, что и дочернее;
                        // поскольку мы находимся в транзакции на сохранение этой карточки,
                        // то никто посторонний гарантированно не может завершить родительское задание,
                        // пока не закончится этот метод

                        // однако задание уже может быть завершено на момент выполнения метода,
                        // поэтому надо проверить его существование перед вставкой строки

                        if (await HasTaskAsync(parentRowID.Value, context.DbScope.Db, builderFactory, context.CancellationToken))
                        {
                            await InsertChildRecordAsync(
                                task, parentRowID.Value, context.StoreDateTime ?? DateTime.UtcNow, executor, builderFactory, context.CancellationToken);

                            await ClearMainTaskInfoAsync(parentRowID.Value, executor, builderFactory, context.CancellationToken);
                        }
                    }
                    else
                    {
                        await UpdateChildRecordAsync(task, executor, builderFactory, context.CancellationToken);
                    }
                }
            }
        }

        #endregion
    }
}