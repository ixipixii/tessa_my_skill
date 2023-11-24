using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ApprovalStageTypeHandler : SubtaskStageTypeHandler
    {
        #region Constructors

        public ApprovalStageTypeHandler(
            IRoleRepository roleRepository,
            ICardMetadata cardMetadata,
            ICardGetStrategy cardGetStrategy,
            ICardCache cardCache,
            IKrScope krScope,
            IBusinessCalendarService calendarService,
            ISession session,
            IStageTasksRevoker tasksRevoker,
            [Dependency(NotificationManagerNames.DeferredWithoutTransaction)]
            INotificationManager notificationManager)
            : base(roleRepository, calendarService, cardMetadata, cardGetStrategy, krScope, tasksRevoker, notificationManager, cardCache, session)
        {
            this.CardCache = cardCache ?? throw new ArgumentNullException(nameof(cardCache));
        }

        #endregion

        #region Protected Properties and Constants

        protected const string TotalPerformerCount = nameof(TotalPerformerCount);
        protected const string CurrentPerformerCount = nameof(CurrentPerformerCount);
        protected const string Disapproved = Keys.Disapproved;

        protected ICardCache CardCache { get; set; }

        #endregion

        #region Protected Methods

        protected virtual StageHandlerResult CompleteApprovalTask(IStageTypeHandlerContext context, bool disapproved)
        {
            var task = context.TaskInfo.Task;

            context.WorkflowAPI.RemoveActiveTask(task.RowID);
            this.RevokeSubTasks(context, task,
                new[]
                {
                    DefaultTaskTypes.KrAdditionalApprovalTypeID,
                    DefaultTaskTypes.KrRequestCommentTypeID
                },
                t =>
                {
                    t.OptionID = t.TypeID == DefaultTaskTypes.KrAdditionalApprovalTypeID
                        ? DefaultCompletionOptions.Revoke
                        : DefaultCompletionOptions.Cancel;
                    t.Result = "$ApprovalHistory_ParentTaskIsCompleted";
                });

            var field = disapproved ? KrApprovalCommonInfo.DisapprovedBy : KrApprovalCommonInfo.ApprovedBy;
            var fields = context.ContextualSatellite.Sections[KrApprovalCommonInfo.Name].Fields;
            var result = StringBuilderHelper.Acquire();

            var value = fields.TryGet<string>(field);
            if (!string.IsNullOrEmpty(value))
            {
                result.Append(value).Append("; ");
            }

            var storeContext = (ICardStoreExtensionContext) context.CardExtensionContext;
            var user = storeContext.Session.User;
            result.Append(user.Name);

            if (task.RoleID != user.ID)
            {
                result.Append(" (").Append(task.RoleName).Append(')');
            }

            fields[field] = result.ToStringAndRelease();

            var stage = context.Stage;
            var total = stage.InfoStorage.Get<int>(TotalPerformerCount);
            var current = stage.InfoStorage.Get<int>(CurrentPerformerCount);
            stage.InfoStorage[CurrentPerformerCount] = Int32Boxes.Box(++current);

            var advisory = stage.SettingsStorage.TryGet<bool?>(KrApprovalSettingsVirtual.Advisory) ?? false;

            if (disapproved)
            {
                stage.InfoStorage[Disapproved] = BooleanBoxes.True;
            }

            var isSetCommentTaskResult =
                disapproved
                || !this.CardCache.Cards
                    .GetAsync(KrSettings.Name).GetAwaiter().GetResult() // TODO async
                    .Sections[KrSettings.Name].RawFields.Get<bool>(KrSettings.HideCommentForApprove);

            var comment =
                isSetCommentTaskResult
                    ? task.Card.Sections[KrTask.Name].Fields.TryGet<string>(KrTask.Comment)
                    : default;

            if (advisory)
            {
                comment = LocalizationManager.Format("$KrProcess_AdvisoryApprovalCommentFormat", comment).TrimEnd();
            }

            if (advisory || isSetCommentTaskResult)
            {
                HandlerHelper.SetTaskResult(context, task, comment);
            }

            var notReturnEdit = (context.ProcessInfo.ProcessParameters.TryGet<bool?>(Keys.NotReturnEdit, true) ?? true)
                && stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.NotReturnEdit);
            var returnWhenDisapproved = stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ReturnWhenDisapproved);
            if (current == total)
            {
                return !advisory && stage.InfoStorage.TryGet<bool>(Disapproved)
                    ? DisapproveAndComplete()
                    : ApproveAndComplete();
            }

            if (!stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.IsParallel))
            {
                if (!advisory && disapproved && returnWhenDisapproved)
                {
                    return DisapproveAndComplete();
                }

                this.SendApprovalTask(context, stage.Performers[current]);
            }

            return StageHandlerResult.InProgressResult;

            StageHandlerResult ApproveAndComplete()
            {
                StagePositionInGroup(out var hasPreviouslyDisapproved, out var hasNext);
                var returnToAuthor = stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ReturnToAuthor);

                if (stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ChangeStateOnEnd))
                {
                    if (hasPreviouslyDisapproved)
                    {
                        this.KrScope.Info[Keys.IgnoreChangeState] = BooleanBoxes.True;
                        context.WorkflowProcess.State = KrState.Disapproved;
                    }
                    else if (!returnToAuthor)
                    {
                        context.WorkflowProcess.State = KrState.Approved;
                    }
                }

                if (notReturnEdit)
                {
                    context.WorkflowProcess.State = KrState.Approved;
                }
                else
                {
                    if (hasPreviouslyDisapproved
                        && !hasNext)
                    {
                        // Последний этап завершен. Этот согласован, но предыдущие могли быть и не согласованы.
                        // Если были несогласованные, то возвращаемся в начало на доработку.
                        return StageHandlerResult.CurrentGroupTransition();
                    }

                    if (returnToAuthor)
                    {
                        // Выполнить переход к этапу "Доработка" расположенному в текущей группе этапов.
                        Guid? editID = null;
                        var cycle = GetCycle(context);
                        context.WorkflowProcess.Stages.ForEachStageInGroup(
                            stage.StageGroupID,
                            currentStage =>
                            {
                                if (currentStage.ID == stage.ID)
                                {
                                    return false;
                                }

                                if (currentStage.StageTypeID == StageTypeDescriptors.EditDescriptor.ID)
                                {
                                    stage.InfoStorage[Interjected] = cycle;
                                    currentStage.InfoStorage[EditStageTypeHandler.ReturnToStage] = stage.ID;
                                    editID = currentStage.ID;
                                    return false;
                                }

                                return true;
                            });

                        // Решарпер считает, что т.к. editID замкнуто в лямбде, то оно может быть изменено в другом потоке
                        // Дадим ему уверенность, переприсвоив в локальную переменную
                        var localEditID = editID;
                        if (localEditID.HasValue)
                        {
                            var transRes = StageHandlerResult.Transition(localEditID.Value, keepStageStates: true);
                            return this.StageCompleted(context, transRes);
                        }

                        context.ValidationResult.AddError(this, "$KrMessages_NoEditStage");
                    }
                }

                return this.StageCompleted(context, StageHandlerResult.CompleteResult);
            }

            StageHandlerResult DisapproveAndComplete()
            {
                StagePositionInGroup(out _, out var hasNext);

                if (!returnWhenDisapproved && hasNext)
                {
                    context.WorkflowProcess.InfoStorage[Disapproved] = BooleanBoxes.True;
                    return this.StageCompleted(context, StageHandlerResult.CompleteResult);
                }

                if (context.Stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Disapproved;
                    this.KrScope.Info[Keys.IgnoreChangeState] = BooleanBoxes.True;
                }

                if (notReturnEdit)
                {
                    return this.StageCompleted(context, StageHandlerResult.CompleteResult);
                }

                var utcNow = DateTime.UtcNow;
                var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[DefaultCompletionOptions.RebuildDocument]; // TODO async
                var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                var userZoneInfo = this.CalendarService.GetRoleTimeZoneInfoAsync(user.ID).GetAwaiter().GetResult(); // TODO async
                var item = new CardTaskHistoryItem
                {
                    State = CardTaskHistoryState.Inserted,
                    RowID = Guid.NewGuid(),
                    TypeID = DefaultTaskTypes.KrRebuildTypeID,
                    TypeName = DefaultTaskTypes.KrRebuildTypeName,
                    TypeCaption = "$CardTypes_TypesNames_KrRebuild",
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
                    GroupRowID = groupID,
                    TimeZoneID = userZoneInfo.TimeZoneID,
                    TimeZoneUtcOffsetMinutes = (int?) userZoneInfo.TimeZoneUtcOffset.TotalMinutes
                };

                this.KrScope.GetMainCard(storeContext.Request.Card.ID).TaskHistory.Add(item);
                context.ContextualSatellite.AddToHistory(
                    item.RowID,
                    context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1));

                return this.StageCompleted(context, StageHandlerResult.GroupTransition(stage.StageGroupID));
            }

            void StagePositionInGroup(out bool hasPreviouslyDisapproved, out bool hasNext)
            {
                var hasPreviouslyDisapprovedClosure = false;
                var hasNextClosure = false;

                var equator = false;
                context.WorkflowProcess.Stages.ForEachStageInGroup(
                    context.Stage.StageGroupID,
                    currStage =>
                    {
                        if (currStage.ID == context.Stage.ID)
                        {
                            equator = true;
                            return;
                        }

                        if (!equator
                            && currStage.StageTypeID == StageTypeDescriptors.ApprovalDescriptor.ID
                            && !(currStage.SettingsStorage.TryGet<bool?>(KrApprovalSettingsVirtual.Advisory) ?? false)
                            && currStage.InfoStorage.TryGet<bool?>(Disapproved) == true)
                        {
                            hasPreviouslyDisapprovedClosure = true;
                        }

                        if (!hasNextClosure
                            && equator
                            && currStage.StageTypeID == StageTypeDescriptors.ApprovalDescriptor.ID)
                        {
                            hasNextClosure = true;
                        }
                    });
                hasPreviouslyDisapproved = hasPreviouslyDisapprovedClosure;
                hasNext = hasNextClosure;
            }
        }


        protected virtual StageHandlerResult StartAdditionalApprovalTask(IStageTypeHandlerContext context)
        {
            var task = context.TaskInfo.Task;

            // TODO: Cтрашный костыль.
            var firstIsResponsible = task.Card.Info
                .Get<bool>(nameof(KrAdditionalApproval.FirstIsResponsible));
            if (firstIsResponsible)
            {
                var isResponsibleSet = task.Card.Sections[nameof(KrAdditionalApprovalInfo)].Rows
                    .Any(u => u.State != CardRowState.Deleted && u.Get<bool>(nameof(KrAdditionalApprovalInfo.IsResponsible)));
                if (isResponsibleSet)
                {
                    context.ValidationResult.AddError(this, "$KrMessages_MoreThenOneResponsible");
                    return StageHandlerResult.EmptyResult;
                }
            }

            var utcNow = DateTime.UtcNow;
            
            // TODO: Cтрашный костыль.
            var comment = task.Card.Info.Get<string>(Comment);
            var plannedDays = task.Card.Info.Get<double>(nameof(KrAdditionalApproval.TimeLimitation));

            var isResponsible = firstIsResponsible;
            var roles = task.Card.Sections[KrAdditionalApprovalUsers.Name].Rows
                .OrderBy(u => u.Get<int>(Order))
                .Select(u => new RoleUser(
                    u.Get<Guid>(KrAdditionalApprovalUsers.RoleID),
                    u.Get<string>(KrAdditionalApprovalUsers.RoleName)));

            var storeContext = (ICardStoreExtensionContext) context.CardExtensionContext;
            var user = storeContext.Session.User;

            foreach (var role in roles)
            {
                this.SendAdditionalApprovalTask(
                    context, task, role.ID, role.Name, isResponsible, utcNow,
                    user.ID, user.Name,
                    comment,
                    plannedDays);

                if (isResponsible)
                {
                    isResponsible = false;
                }
            }

            return StageHandlerResult.InProgressResult;
        }


        protected virtual StageHandlerResult CompleteAdditionalApprovalTask(IStageTypeHandlerContext context, bool disapproved)
        {
            var task = context.TaskInfo.Task;

            context.WorkflowAPI.RemoveActiveTask(task.RowID);
            this.SubTaskCompleted(context);
            this.RevokeSubTasks(context, task,
                new[] { DefaultTaskTypes.KrRequestCommentTypeID },
                t => t.Result = "$ApprovalHistory_ParentTaskIsCompleted");

            var parentTaskID = task.ParentRowID;
            if (parentTaskID == null)
            {
                return StageHandlerResult.EmptyResult;
            }

            var taskComment = context
                .TaskInfo
                .Task
                .Card
                .Sections[KrAdditionalApprovalTaskInfo.Name]
                .RawFields
                .Get<string>(Comment);
            HandlerHelper.SetTaskResult(context, task, taskComment);

            Guid? approverID;
            Guid? responsibleID;
            int notCompleted;

            var storeContext = (ICardStoreExtensionContext) context.CardExtensionContext;
            var scope = storeContext.DbScope;
            using (scope.Create())
            {
                approverID = scope.Db
                    .SetCommand(
                        scope.BuilderFactory
                            .Select().C("UserID")
                            .From("Tasks").NoLock()
                            .Where().C("RowID").Equals().P("RowID")
                            .Build(),
                        scope.Db.Parameter("RowID", parentTaskID.Value))
                    .LogCommand()
                    .Execute<Guid?>();

                var isResponsible = task.Card.Sections[nameof(KrAdditionalApprovalTaskInfo)].Fields
                    .Get<bool>(nameof(KrAdditionalApprovalTaskInfo.IsResponsible));

                responsibleID = isResponsible
                    ? scope.Db
                        .SetCommand(
                            scope.BuilderFactory
                                .Select().C("t", "UserID")
                                .From("Tasks", "t").NoLock()
                                .InnerJoin("KrAdditionalApprovalInfo", "i").NoLock()
                                .On().C("i", "RowID").Equals().C("t", "RowID")
                                .Where().C("i", "ID").Equals().P("RowID")
                                .And().C("i", "IsResponsible").Equals().V(true)
                                .Build(),
                            scope.Db.Parameter("RowID", parentTaskID.Value))
                        .LogCommand()
                        .Execute<Guid?>()
                    : null;

                notCompleted = scope.Db
                    .SetCommand(
                        scope.BuilderFactory
                            .Select().Count().Substract(1)
                            .From("KrAdditionalApprovalInfo").NoLock()
                            .Where().C("ID").Equals().P("RowID")
                            .And().C("Completed").IsNull()
                            .Build(),
                        scope.Db.Parameter("RowID", parentTaskID.Value))
                    .LogCommand()
                    .Execute<int>();
            }

            var roleList = new List<Guid>();
            if (approverID.HasValue)
            {
                roleList.Add(approverID.Value);
            }

            if (responsibleID.HasValue)
            {
                roleList.Add(responsibleID.Value);
            }

            if (roleList.Count > 0)
            {
                var isCompleted = notCompleted == 0;
                var cardID = context.MainCardID ?? Guid.Empty;
                context.ValidationResult.Add(
                    NotificationManager
                        .SendAsync(
                            isCompleted
                                ? DefaultNotifications.AdditionalApprovalNotificationCompleted
                                : DefaultNotifications.AdditionalApprovalNotification,
                            roleList,
                            new NotificationSendContext()
                            {
                                MainCardID = cardID,
                                Info = Shared.Notices.NotificationHelper.GetInfoWithTask(task),
                                GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                                ModifyEmailActionAsync = async (email, ct) =>
                                {
                                    if (!isCompleted)
                                    {
                                        email.PlaceholderAliases.SetReplacement(
                                            "subjectLabel",
                                            disapproved
                                                ? "$DisapprovedAdditionalApprovalNotificationTemplate_SubjectLabel"
                                                : "$ApprovedAdditionalApprovalNotificationTemplate_SubjectLabel");
                                        email.PlaceholderAliases.SetReplacement(
                                            "taskCount",
                                            $"text:{notCompleted}");
                                    }

                                    email.PlaceholderAliases.SetReplacement(
                                        "resultLabel",
                                        disapproved
                                            ? "$DisapprovedNotificationTemplate_BodyLabel"
                                            : "$ApprovedNotificationTemplate_BodyLabel");
                                }
                            }).GetAwaiter().GetResult()); // TODO async
            }

            return StageHandlerResult.InProgressResult;
        }


        protected virtual void SendApprovalTask(
            IStageTypeHandlerContext context,
            Performer performer)
        {
            var roleName = KrAdditionalApprovalMarker.Unmark(performer.PerformerName);
            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return;
            }

            var authorID = author.AuthorID;
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);

            var approvalTask = this.SendTask(
                context,
                DefaultTaskTypes.KrApproveTypeID,
                this.GetTaskDigest(context),
                performer.PerformerID,
                roleName,
                t =>
                {
                    t.AuthorID = authorID;
                    t.AuthorName = null; // AuthorName и AuthorPosition определяются системой, когда явно указано null
                    t.GroupRowID = groupID;
                    t.Planned = context.Stage.Planned;
                    t.PlannedQuants = context.Stage.PlannedQuants;
                    HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                });

            var created = DateTime.UtcNow;
            var performerRowID = performer.RowID;
            var aa = context.Stage.SettingsStorage.TryGet<List<object>>(KrAdditionalApprovalUsersCardVirtual.Synthetic);
            if (aa != null)
            {
                foreach (var row in aa)
                {
                    if (row is Dictionary<string, object> additional &&
                        additional.Get<Guid>(nameof(KrAdditionalApprovalUsersCardVirtual.MainApproverRowID)) == performerRowID)
                    {
                        this.SendAdditionalApprovalTask(
                            context,
                            approvalTask,
                            additional.Get<Guid>(nameof(KrAdditionalApprovalUsersCardVirtual.RoleID)),
                            additional.Get<string>(nameof(KrAdditionalApprovalUsersCardVirtual.RoleName)),
                            additional.Get<bool>(nameof(KrAdditionalApprovalUsersCardVirtual.IsResponsible)),
                            created,
                            approvalTask.AuthorID,
                            approvalTask.AuthorName,
                            null);
                    }
                }
            }
        }


        protected virtual void SendAdditionalApprovalTask(IStageTypeHandlerContext context,
            CardTask approvalTask,
            Guid roleID,
            string roleName,
            bool isResponsible,
            DateTime created,
            Guid? authorID,
            string authrName,
            string comment,
            double? plannedDays = null)
        {
            var storeContext = (ICardStoreExtensionContext) context.CardExtensionContext;
            var user = storeContext.Session.User;

            // Временная зона текущего сотрудника, для записи в историю заданий
            var userZoneInfo = this.CalendarService.GetRoleTimeZoneInfoAsync(user.ID).GetAwaiter().GetResult(); // TODO async
            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[DefaultCompletionOptions.AdditionalApproval]; // TODO async
            var infoItem = new CardTaskHistoryItem
            {
                State = CardTaskHistoryState.Inserted,
                ParentRowID = approvalTask.RowID,
                RowID = Guid.NewGuid(),
                TypeID = DefaultTaskTypes.KrInfoAdditionalApprovalTypeID,
                TypeName = DefaultTaskTypes.KrInfoAdditionalApprovalTypeName,
                TypeCaption = "$CardTypes_TypesNames_KrAdditionalApproval",
                Created = created,
                Planned = created,
                InProgress = created,
                Completed = created,
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
                GroupRowID = groupID,
                Result = comment ?? (isResponsible
                    ? "{$KrMessages_ResponsibleAdditionalApprovalComment}"
                    : "{$KrMessages_DefaultAdditionalApprovalComment}"),
                TimeZoneID = userZoneInfo.TimeZoneID,
                TimeZoneUtcOffsetMinutes = (int?) userZoneInfo.TimeZoneUtcOffset.TotalMinutes
            };

            var cycle = context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1);
            var history = this.KrScope.GetMainCard(storeContext.Request.Card.ID).TaskHistory;
            var satellite = context.ContextualSatellite;

            history.Add(infoItem);
            satellite.AddToHistory(infoItem.RowID, cycle);

            var authorComment = context.WorkflowProcess.AuthorComment;
            var digest = string.IsNullOrWhiteSpace(authorComment)
                ? comment
                : comment + Environment.NewLine + authorComment;

            if (isResponsible)
            {
                digest = string.IsNullOrWhiteSpace(digest)
                    ? "{$KrMessages_ResponsibleAdditionalDigest}"
                    : "{$KrMessages_ResponsibleAdditionalDigest}." + Environment.NewLine + digest;
            }

            var additionalTask =
                this.SendSubTask(
                    context,
                    approvalTask,
                    DefaultTaskTypes.KrAdditionalApprovalTypeID,
                    digest,
                    roleID,
                    roleName,
                    modifyTask: t =>
                    {
                        t.AuthorID = authorID;
                        t.AuthorName = authrName;
                        t.ParentRowID = approvalTask.RowID;
                        t.Planned = null;
                        t.HistoryItemParentRowID = infoItem.RowID;
                        t.GroupRowID = groupID;

                        if (plannedDays.HasValue)
                        {
                            t.Planned = null;
                            t.PlannedQuants = (int) (plannedDays * 32.0); // округление до кванта в меньшую сторону
                        }
                        else
                        {
                            t.Planned = context.Stage.Planned;
                            t.PlannedQuants = context.Stage.PlannedQuants;
                        }
                    },
                    createHistory: true);

            var additionalCard = additionalTask.Card;
            additionalCard.Sections[nameof(TaskCommonInfo)].Fields[nameof(Info)] = comment;
            additionalCard.Sections[nameof(KrAdditionalApprovalTaskInfo)].Fields[nameof(KrAdditionalApprovalTaskInfo.AuthorRoleID)] = approvalTask.RoleID;
            additionalCard.Sections[nameof(KrAdditionalApprovalTaskInfo)].Fields[nameof(KrAdditionalApprovalTaskInfo.AuthorRoleName)] = approvalTask.RoleName;
            additionalCard.Sections[nameof(KrAdditionalApprovalTaskInfo)].Fields[nameof(KrAdditionalApprovalTaskInfo.IsResponsible)] = isResponsible;
        }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            if (IsInterjected(context))
            {
                if (stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Approved;
                }

                stage.InfoStorage.Remove(Interjected);
                return StageHandlerResult.CompleteResult;
            }

            var baseResult = base.HandleStageStart(context);
            if (baseResult != StageHandlerResult.EmptyResult)
            {
                return baseResult;
            }

            var performers = stage.Performers;
            if (performers.Count == 0)
            {
                if (stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Approved;
                }

                return StageHandlerResult.CompleteResult;
            }

            if (stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.ChangeStateOnStart))
            {
                context.WorkflowProcess.State = KrState.Active;
            }

            stage.InfoStorage[TotalPerformerCount] = Int32Boxes.Box(performers.Count);
            stage.InfoStorage[CurrentPerformerCount] = Int32Boxes.Zero;
            stage.InfoStorage[Disapproved] = BooleanBoxes.False;

            if (stage.SettingsStorage.Get<bool>(KrApprovalSettingsVirtual.IsParallel))
            {
                foreach (var performer in performers)
                {
                    this.SendApprovalTask(context, performer);
                }
            }
            else
            {
                this.SendApprovalTask(context, performers[0]);
            }

            return StageHandlerResult.InProgressResult;
        }


        public override StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            var baseResult = base.HandleTaskCompletion(context);
            if (baseResult != StageHandlerResult.EmptyResult)
            {
                return baseResult;
            }

            var task = context.TaskInfo.Task;
            if (task.TypeID == DefaultTaskTypes.KrAdditionalApprovalTypeID)
            {
                var optionID = task.OptionID;
                HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
                if (optionID == DefaultCompletionOptions.Approve)
                {
                    return this.CompleteAdditionalApprovalTask(context, false);
                }

                if (optionID == DefaultCompletionOptions.Disapprove)
                {
                    return this.CompleteAdditionalApprovalTask(context, true);
                }

                if (optionID == DefaultCompletionOptions.Revoke)
                {
                    this.RevokeSubTasks(context, task,
                        new[] { DefaultTaskTypes.KrRequestCommentTypeID },
                        t => t.Result = "$ApprovalHistory_ParentTaskIsCompleted");
                    return this.SubTaskCompleted(context);
                }
            }
            else
            {
                var optionID = task.OptionID;
                HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
                if (optionID == DefaultCompletionOptions.Approve)
                {
                    return this.CompleteApprovalTask(context, false);
                }

                if (optionID == DefaultCompletionOptions.Disapprove)
                {
                    return this.CompleteApprovalTask(context, true);
                }

                if (optionID == DefaultCompletionOptions.AdditionalApproval)
                {
                    return this.StartAdditionalApprovalTask(context);
                }
            }

            return StageHandlerResult.EmptyResult;
        }


        public override bool HandleStageInterrupt(IStageTypeHandlerContext context)
            => this.HandleStageInterrupt(context,
                new[]
                {
                    DefaultTaskTypes.KrApproveTypeID,
                    DefaultTaskTypes.KrAdditionalApprovalTypeID,
                    DefaultTaskTypes.KrRequestCommentTypeID
                },
                t => t.Result = "$ApprovalHistory_TaskCancelled");


        protected override void HandleTaskDelegate(IStageTypeHandlerContext context, CardTask delegatedTask)
        {
            var task = context.TaskInfo.Task;
            if (task.Card.Sections.TryGetValue(nameof(KrAdditionalApprovalInfo), out var oldSection))
            {
                var newSection = new CardSection(nameof(KrAdditionalApprovalInfo), oldSection.GetStorage())
                {
                    Type = CardSectionType.Table
                };

                foreach (var row in newSection.Rows)
                {
                    row.Fields["ID"] = delegatedTask.RowID;
                    row.State = CardRowState.Inserted;
                }

                delegatedTask.Card.Sections[nameof(KrAdditionalApprovalInfo)].Set(newSection);
            }

            var storeContext = (ICardStoreExtensionContext) context.CardExtensionContext;
            storeContext.Request.Info.Add(KrReassignAdditionalApprovalStoreExtension.ReassignTo, delegatedTask.RowID);
        }

        #endregion
    }
}