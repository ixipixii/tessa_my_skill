using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Data;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;
using NotificationHelper = Tessa.Extensions.Default.Shared.Notices.NotificationHelper;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public abstract class SubtaskStageTypeHandler : StageTypeHandlerBase
    {
        #region Fields

        private static readonly Guid[] subTaskTypeIDs = { DefaultTaskTypes.KrRequestCommentTypeID };

        private readonly ICardCache cardCache;

        #endregion

        #region Constructors

        protected SubtaskStageTypeHandler(
            IRoleRepository roleRepository,
            IBusinessCalendarService calendarService,
            ICardMetadata cardMetadata,
            ICardGetStrategy cardGetStrategy,
            IKrScope krScope,
            IStageTasksRevoker tasksRevoker,
            INotificationManager notificationManager,
            ICardCache cardCache,
            ISession session)
        {
            this.RoleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this.CalendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
            this.CardMetadata = cardMetadata ?? throw new ArgumentNullException(nameof(cardMetadata));
            this.CardGetStrategy = cardGetStrategy ?? throw new ArgumentNullException(nameof(cardGetStrategy));
            this.KrScope = krScope ?? throw new ArgumentNullException(nameof(krScope));
            this.TasksRevoker = tasksRevoker ?? throw new ArgumentNullException(nameof(tasksRevoker));
            this.NotificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
            this.cardCache = cardCache ?? throw new ArgumentNullException(nameof(cardCache));
            this.Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        #endregion

        #region Protected Properties and Constants

        protected const string SubtaskCount = nameof(SubtaskCount);
        protected const string ResultAction = nameof(ResultAction);
        protected const string ResultTransitTo = nameof(ResultTransitTo);
        protected const string ResultKeepStates = nameof(ResultKeepStates);
        protected const string Interjected = nameof(Interjected);

        protected IRoleRepository RoleRepository { get; set; }
        protected IBusinessCalendarService CalendarService { get; set; }
        protected ICardMetadata CardMetadata { get; set; }
        protected ICardGetStrategy CardGetStrategy { get; set; }
        protected IKrScope KrScope { get; set; }

        protected IStageTasksRevoker TasksRevoker { get; }
        protected INotificationManager NotificationManager { get; }
        protected ISession Session { get; set; }

        #endregion

        #region Protected Methods

        protected virtual Guid[] GetSubTaskTypesToRevoke() => subTaskTypeIDs;

        protected virtual string GetTaskDigest(IStageTypeHandlerContext context, string additionalComment = null)
        {
            var builder = new StringBuilder()
                .Append("{$KrMessages_Stage}: ")
                .Append(context.Stage.Name);

            var stageComment = context.Stage.SettingsStorage.Get<string>(KrApprovalSettingsVirtual.Comment);
            if (!string.IsNullOrEmpty(stageComment))
            {
                builder.Append(". ").Append(stageComment);
            }

            var authorComment = context.WorkflowProcess.AuthorComment;
            if (!string.IsNullOrEmpty(authorComment))
            {
                builder
                    .AppendLine()
                    .AppendLine("---------------")
                    .Append(authorComment);
            }

            if (!string.IsNullOrEmpty(additionalComment))
            {
                builder
                    .AppendLine()
                    .AppendLine("---------------")
                    .Append(additionalComment);
            }


            return builder.ToString();
        }


        protected virtual void HandleTaskDelegate(IStageTypeHandlerContext context, CardTask delegatedTask)
        {
        }


        protected virtual bool HandleStageInterrupt(IStageTypeHandlerContext context, Guid[] taskTypeIDs, Action<CardTask> revoke)
        {
            context.Stage.InfoStorage.Remove(SubtaskCount);
            return this.RevokeTasks(context, taskTypeIDs, revoke, removeFromActive: true) == 0;
        }


        protected CardTask SendTask(IStageTypeHandlerContext context, Guid typeID, string digest, Performer performer, Action<CardTask> modifyTask = null, bool createHistory = true)
            => this.SendTask(context, typeID, digest, performer.PerformerID, performer.PerformerName, modifyTask, createHistory);

        protected virtual CardTask SendTask(IStageTypeHandlerContext context, Guid typeID, string digest, Guid performerID, string performerName, Action<CardTask> modifyTask = null, bool createHistory = true)
        {
            var task = context.WorkflowAPI.SendTask(typeID, digest, performerID, performerName, modifyTaskAction: modifyTask).Task;
            if (createHistory)
            {
                var advisory = context.Stage.SettingsStorage.TryGet<bool?>(KrApprovalSettingsVirtual.Advisory) ?? false;
                task.Flags |= CardTaskFlags.CreateHistoryItem;
                context.ContextualSatellite.AddToHistory(
                    task.RowID,
                    context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1),
                    advisory);
            }

            context.WorkflowAPI.AddActiveTask(task.RowID);

            context.ValidationResult.Add(
                NotificationManager.SendAsync(
                    DefaultNotifications.TaskNotification,
                    new[] { task.RoleID },
                    new NotificationSendContext()
                    {
                        MainCardID = context.MainCardID ?? Guid.Empty,
                        Info = NotificationHelper.GetInfoWithTask(task),
                        ModifyEmailActionAsync = async (email, ct) =>
                        {
                            NotificationHelper.ModifyEmailForMobileApprovers(
                                email,
                                task,
                                await NotificationHelper.GetMobileApprovalEmailAsync(cardCache, ct));

                            NotificationHelper.ModifyTaskCaption(
                                email,
                                task);
                        },
                        GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                    }).GetAwaiter().GetResult()); // TODO async

            return task;
        }


        protected virtual CardTask SendSubTask(IStageTypeHandlerContext context, CardTask parentTask, Guid typeID, string digest, Guid performerID, string performerName, Action<CardTask> modifyTask = null, bool createHistory = true)
        {
            var task = this.SendTask(context, typeID, digest, performerID, performerName, modifyTask, createHistory);
            task.ParentRowID = parentTask.RowID;

            var info = context.Stage.InfoStorage;
            var count = info.TryGet<int>(SubtaskCount) + 1;
            info[SubtaskCount] = Int32Boxes.Box(count);

            return task;
        }


        protected virtual int RevokeTasks(IStageTypeHandlerContext context, Guid[] taskTypeIDs, Action<CardTask> revoke, bool removeFromActive = false)
        {
            var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
            var scope = storeContext.DbScope;
            using (scope.Create())
            {
                var db = scope.Db;
                var query = scope.BuilderFactory
                    .Select().C("t", "RowID")
                    .From("Tasks", "t").NoLock()
                    .InnerJoin("WorkflowTasks", "wt").NoLock()
                        .On().C("wt", "RowID").Equals().C("t", "RowID")
                    .InnerJoin("WorkflowProcesses", "wp").NoLock()
                        .On().C("wp", "RowID").Equals().C("wt", "ProcessRowID")
                    .Where().C("t", "TypeID").Q(" IN (");

                var index = 0;
                var parameters = new DataParameter[taskTypeIDs.Length + 2];
                while (index < taskTypeIDs.Length)
                {
                    var parameter = db.Parameter($"TypeID{index}", taskTypeIDs[index]);
                    query.Parameter(parameter.Name);
                    parameters[index++] = parameter;
                }

                parameters[index++] = db.Parameter("SatelliteID", context.ProcessHolderSatellite.ID);
                parameters[index] = db.Parameter("ProcessID", context.ProcessInfo.ProcessID);

                var tasksToRevoke = db
                    .SetCommand(
                        query.Q(")")
                            .And().C("wt", "ID").Equals().P("SatelliteID")
                            .And().C("wp", "RowID").Equals().P("ProcessID")
                            .Build(),
                        parameters)
                    .LogCommand()
                    .ExecuteList<Guid>();
                return this.RevokeTasksCore(context, tasksToRevoke, revoke, removeFromActive);
            }
        }


        protected virtual int RevokeSubTasks(IStageTypeHandlerContext context, CardTask parentTask, Guid[] taskTypeIDs, Action<CardTask> revoke, bool removeFromActive = false)
        {
            var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
            var scope = storeContext.DbScope;
            using (scope.Create())
            {
                var db = scope.Db;
                var query = scope.BuilderFactory
                    .Select().C("RowID")
                    .From("Tasks").NoLock()
                    .Where().C("TypeID").Q(" IN (");

                var index = 0;
                var parameters = new DataParameter[taskTypeIDs.Length + 1];
                while (index < taskTypeIDs.Length)
                {
                    var parameter = db.Parameter($"TypeID{index}", taskTypeIDs[index]);
                    query.Parameter(parameter.Name);
                    parameters[index++] = parameter;
                }

                parameters[index] = db.Parameter("ParentApprovalID", parentTask.RowID);

                var tasksToRevoke = db
                    .SetCommand(
                        query.Q(")")
                            .And().C("ParentID").Equals().P("ParentApprovalID")
                            .Build(),
                        parameters)
                    .LogCommand()
                    .ExecuteList<Guid>();
                return this.RevokeTasksCore(context, tasksToRevoke, revoke, removeFromActive);
            }
        }


        protected virtual int RevokeTasksCore(
            IStageTypeHandlerContext context,
            List<Guid> tasksToRevoke,
            Action<CardTask> revoke,
            bool removeFromActive) =>
            this.TasksRevoker.RevokeTasks(new StageTaskRevokerContext(context)
            {
                CardID = context.MainCardID ?? Guid.Empty,
                TaskIDs = tasksToRevoke,
                RemoveFromActive = removeFromActive,
                TaskModificationAction = task =>
                {
                    const string revokedByParent = CardHelper.SystemKeyPrefix + "revokedByParent";
                    task.Info[revokedByParent] = BooleanBoxes.True;
                    revoke(task);
                }
            });

        protected virtual StageHandlerResult SubTaskCompleted(IStageTypeHandlerContext context)
        {
            var info = context.Stage.InfoStorage;
            var count = info.Get<int>(SubtaskCount) - 1;
            info[SubtaskCount] = Int32Boxes.Box(count);

            if (count == 0 && info.TryGetValue(ResultAction, out var actionUntyped))
            {
                var action = (StageHandlerAction)(int)actionUntyped;
                var transitTo = info.TryGet<Guid?>(ResultTransitTo);
                var keepStates = info.TryGet<bool?>(ResultKeepStates);

                info.Remove(ResultAction);
                info.Remove(ResultTransitTo);
                info.Remove(ResultKeepStates);

                return new StageHandlerResult(action, transitTo, keepStates);
            }

            return StageHandlerResult.InProgressResult;
        }


        protected virtual StageHandlerResult StageCompleted(IStageTypeHandlerContext context, StageHandlerResult result)
        {
            var info = context.Stage.InfoStorage;
            if (info.TryGet<int>(SubtaskCount) == 0)
            {
                return result;
            }

            info.Add(ResultAction, (int)result.Action);

            var transitTo = result.TransitionID;
            if (transitTo.HasValue)
            {
                info.Add(ResultTransitTo, transitTo.Value);
            }

            var keepStates = result.KeepStageStates;
            if (keepStates.HasValue)
            {
                info.Add(ResultKeepStates, keepStates.Value);
            }

            return StageHandlerResult.InProgressResult;
        }

        protected static int GetCycle(IStageTypeHandlerContext context) => context.WorkflowProcess.InfoStorage.TryGet<int>(Keys.Cycle);

        protected static bool IsInterjected(
            IStageTypeHandlerContext context)
        {
            if (!context.Stage.InfoStorage.TryGetValue(Interjected, out var interjected))
            {
                return false;
            }

            switch (interjected)
            {
                // Легаси, когда хранился только флажок
                case bool interjectedBool:
                    return interjectedBool;
                // Хранится цикл интерджекта, чтобы не было пропуска этапа при вернулось->доработка->отзыв->запуск процесса
                case int interjectedInt:
                    var cycle = GetCycle(context);
                    return interjectedInt == cycle;
                case null:
                    return false;
                default:
                    throw new InvalidOperationException($"Invalid value of interjected key in approval stage {interjected}");
            }
        }

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override void BeforeInitialization(IStageTypeHandlerContext context)
        {
            HandlerHelper.ClearCompletedTasks(context.Stage);
        }

        public override StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            var task = context.TaskInfo.Task;
            var optionID = task.OptionID;
            if (optionID == DefaultCompletionOptions.RequestComments)
            {
                HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
                var roleUser = CreateTaskRole(task.Card.Sections[KrCommentators.Name].Rows);
                if (roleUser.HasValue)
                {
                    // TODO: страшные костыли. Расширения KrClearWasteInApprovalStoreTaskExtension и KrClearWasteInAdditionalApprovalStoreTaskExtension
                    // очищает значения полей секции KrTask, поэтому используется Info, куда они
                    // переносятся ради возможности их получить в данном обработчике.
                    var comment = task.Card.Info.TryGet<string>(nameof(KrTask.Comment));
                    var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
                    var utcNow = DateTime.UtcNow;
                    var user = storeContext.Session.User;
                    var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[DefaultCompletionOptions.RequestComments]; // TODO async
                    var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                    // Временная зона текущего сотрудника, для записи в историю заданий
                    var userZoneInfo = this.CalendarService.GetRoleTimeZoneInfoAsync(user.ID).GetAwaiter().GetResult(); // TODO async
                    var questionItem = new CardTaskHistoryItem
                    {
                        State = CardTaskHistoryState.Inserted,
                        ParentRowID = task.RowID,
                        RowID = Guid.NewGuid(),
                        TypeID = DefaultTaskTypes.KrInfoRequestCommentTypeID,
                        TypeName = DefaultTaskTypes.KrInfoRequestCommentTypeName,
                        TypeCaption = "$CardTypes_TypesNames_KrInfoRequestComment",
                        Created = utcNow,
                        Planned = utcNow,
                        InProgress = utcNow,
                        Completed = utcNow,
                        CompletedByID = user.ID,
                        CompletedByName = user.Name,
                        AuthorID = user.ID,
                        AuthorName = user.Name,
                        UserID = user.ID,
                        UserName = user.Name,
                        RoleID = user.ID,
                        RoleName = user.Name,
                        RoleTypeID = RoleHelper.PersonalRoleTypeID,
                        OptionID = option.ID,
                        OptionName = option.Name,
                        OptionCaption = option.Caption,
                        Result = comment,
                        GroupRowID = groupID,
                        TimeZoneID = userZoneInfo.TimeZoneID,
                        TimeZoneUtcOffsetMinutes = (int?)userZoneInfo.TimeZoneUtcOffset.TotalMinutes
                    };

                    var cycle = context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1);
                    var history = this.KrScope.GetMainCard(storeContext.Request.Card.ID).TaskHistory;
                    var satellite = context.ContextualSatellite;

                    history.Add(questionItem);
                    satellite.AddToHistory(questionItem.RowID, cycle);

                    var answerTask = this.SendSubTask(
                        context,
                        task,
                        DefaultTaskTypes.KrRequestCommentTypeID,
                        comment,
                        roleUser.Value.ID,
                        roleUser.Value.Name,
                        modifyTask: t =>
                        {
                            t.ParentRowID = task.RowID;
                            t.HistoryItemParentRowID = questionItem.RowID;
                            t.GroupRowID = groupID;
                        },
                        createHistory: true);

                    var card = answerTask.Card;
                    card.Sections[TaskCommonInfo.Name].Fields[TaskCommonInfo.Info] = answerTask.Digest;
                    card.Sections[KrRequestComment.Name].Fields[KrRequestComment.AuthorRoleID] = task.UserID;
                    card.Sections[KrRequestComment.Name].Fields[KrRequestComment.AuthorRoleName] = task.UserName;
                }
                else
                {
                    context.ValidationResult.AddError(this, "$KrMessages_NeedToSpecifyRespondent");
                }

                return StageHandlerResult.InProgressResult;

                RoleUser? CreateTaskRole(IEnumerable<CardRow> commentators)
                {
                    var users = commentators
                        .Select(c => c.TryGetValue(KrCommentators.CommentatorID, out var id) && c.TryGetValue(KrCommentators.CommentatorName, out var name)
                            ? new RoleUser((Guid)id, (string)name) : new RoleUser?())
                        .Where(c => c.HasValue)
                        .Select(c => c.Value)
                        .ToArray();
                    if (users.Length == 0)
                    {
                        return null;
                    }

                    if (users.Length == 1)
                    {
                        return users[0];
                    }

                    var taskRole = RoleHelper.CreateTaskRole(users);

                    // TODO async
                    this.RoleRepository.InsertAsync(taskRole).GetAwaiter().GetResult();
                    return new RoleUser(taskRole.ID, taskRole.Name);
                }
            }

            if (task.TypeID == DefaultTaskTypes.KrRequestCommentTypeID)
            {
                if (optionID == DefaultCompletionOptions.AddComment ||
                    optionID == DefaultCompletionOptions.Cancel)
                {
                    if (optionID == DefaultCompletionOptions.AddComment)
                    {
                        var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
                        var comment = task.Card.Sections[KrRequestComment.Name].Fields.TryGet<string>(Comment);
                        HandlerHelper.SetTaskResult(context, task, comment);

                        context.ValidationResult.Add(
                            NotificationManager
                                .SendAsync(
                                    DefaultNotifications.CommentNotification,
                                    new Guid[] { task.AuthorID ?? Guid.Empty },
                                    new NotificationSendContext()
                                    {
                                        MainCardID = context.MainCardID ?? Guid.Empty,
                                        GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                                        Info = NotificationHelper.GetInfoWithTask(task),
                                    }).GetAwaiter().GetResult()); // TODO async
                    }

                    // Таск завершается без дальнейшей обработки.
                    // Обновление родительского таска в расширении KrUpdateParentTaskExtension.
                    context.WorkflowAPI.RemoveActiveTask(task.RowID);
                    return this.SubTaskCompleted(context);
                }
            }

            if (optionID == DefaultCompletionOptions.Delegate)
            {
                HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
                var fields = task.Card.Sections[nameof(KrTask)].Fields;

                // Новый StringBuilder по той причине, что длина строк
                // может быть достаточно большой и StringBuilder не будет
                // возвращаться обратно в StringBuilderHelper.
                var result = new StringBuilder()
                    .Append("{$ApprovalHistory_TaskIsDelegated} \"")
                    .Append(fields.Get<string>(nameof(KrTask.DelegateName)))
                    .Append('"');

                string digest;
                string comment = fields.TryGet<string>(nameof(KrTask.Comment));

                if (string.IsNullOrWhiteSpace(comment))
                {
                    digest = this.GetTaskDigest(context);
                }
                else
                {
                    digest = this.GetTaskDigest(context, comment);

                    result
                        .Append(". {$ApprovalHistory_Comment}: ")
                        .Append(comment);
                }

                context.WorkflowAPI.TryRemoveActiveTask(task.RowID);
                HandlerHelper.SetTaskResult(context, task, result.ToString());
                this.RevokeSubTasks(context, task, this.GetSubTaskTypesToRevoke(), t =>
                {
                    t.OptionID = DefaultCompletionOptions.Cancel;
                    t.Result = "$ApprovalHistory_ParentTaskIsCompleted";
                });
                var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);

                var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
                var authorID = author.AuthorID;
                this.SendTask(
                    context,
                    task.TypeID,
                    digest,
                    fields.Get<Guid>(nameof(KrTask.DelegateID)),
                    fields.Get<string>(nameof(KrTask.DelegateName)),
                    t =>
                    {
                        t.AuthorID = authorID;
                        t.AuthorName = null; // AuthorName и AuthorPosition определяются системой, когда явно указано null
                        t.Planned = task.Planned;
                        t.ParentRowID = task.RowID;
                        t.HistoryItemParentRowID = task.RowID;
                        t.GroupRowID = groupID;
                        HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);

                        this.HandleTaskDelegate(context, t);
                    });

                return StageHandlerResult.InProgressResult;
            }

            return StageHandlerResult.EmptyResult;
        }


        public override bool HandleStageInterrupt(IStageTypeHandlerContext context) => true;

        #endregion
    }
}