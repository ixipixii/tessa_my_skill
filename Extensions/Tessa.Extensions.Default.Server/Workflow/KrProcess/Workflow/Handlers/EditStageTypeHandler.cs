using System;
using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;
using NotificationHelper = Tessa.Extensions.Default.Shared.Notices.NotificationHelper;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class EditStageTypeHandler : StageTypeHandlerBase
    {
        #region Constants

        public const string ReturnToStage = nameof(ReturnToStage);

        #endregion

        #region Constructors

        public EditStageTypeHandler(
            IKrScope krScope,
            IBusinessCalendarService calendarService,
            ISession session,
            IRoleRepository roleRepository,
            IStageTasksRevoker tasksRevoker,
            [Dependency(NotificationManagerNames.DeferredWithoutTransaction)] INotificationManager notificationManager,
            ICardCache cardCache)
        {
            this.KrScope = krScope ?? throw new ArgumentNullException(nameof(krScope));
            this.CalendarService = calendarService;
            this.Session = session;
            this.RoleRepository = roleRepository;
            this.TasksRevoker = tasksRevoker;
            this.NotificationManager = notificationManager;
            this.CardCache = cardCache;
        }

        #endregion

        #region Protected Properties

        protected IKrScope KrScope { get; }
        protected IBusinessCalendarService CalendarService { get; }
        protected ISession Session { get; }
        protected IRoleRepository RoleRepository { get; }
        protected IStageTasksRevoker TasksRevoker { get; }
        protected INotificationManager NotificationManager { get; }
        protected ICardCache CardCache { get; }

        #endregion

        #region Protected Methods

        protected virtual StageHandlerResult StartApproval(IStageTypeHandlerContext context, StageHandlerResult? result = null)
        {
            var returnToStage = context.Stage.InfoStorage.TryGet<Guid?>(ReturnToStage);
            if (returnToStage.HasValue)
            {
                context.Stage.InfoStorage.Remove(ReturnToStage);
                return StageHandlerResult.Transition(returnToStage.Value, keepStageStates: true);
            }

            var fields = context.ContextualSatellite.Sections[KrApprovalCommonInfo.Name].Fields;
            fields[KrApprovalCommonInfo.ApprovedBy] = string.Empty;
            fields[KrApprovalCommonInfo.DisapprovedBy] = string.Empty;

            return result ?? StageHandlerResult.CompleteResult;
        }


        protected virtual void TryIncrementCycle(IStageTypeHandlerContext context)
        {
            var info = context.WorkflowProcess.InfoStorage;
            var cycle = info.TryGet<int>(Keys.Cycle);
            var settings = context.Stage.SettingsStorage;
            if (settings.TryGet<bool?>(KrEditSettingsVirtual.IncrementCycle) == true)
            {
                info[Keys.Cycle] = Int32Boxes.Box(++cycle);
            }
        }


        protected virtual bool TransitFromDifferentGroup(Stage stage, Guid? cardID)
        {
            var trace = this.KrScope.GetKrProcessRunnerTrace();

            if (trace is null
                || trace.Count == 0)
            {
                return true;
            }

            for (var i = trace.Count - 1; 0 <= i; i--)
            {
                var traceItem = trace[i];
                var prevStage = traceItem.Stage;
                if (prevStage.StageGroupID != stage.StageGroupID
                    || traceItem.CardID != cardID)
                {
                    return true;
                }
                if (!prevStage.Hidden)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Base Overrides
        
        /// <inheritdoc />
        public override void BeforeInitialization(IStageTypeHandlerContext context)
        {
            HandlerHelper.ClearCompletedTasks(context.Stage);
        }

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            var settings = stage.SettingsStorage;
            var returnToStage = stage.InfoStorage.ContainsKey(ReturnToStage);
            if (!returnToStage)
            {
                this.TryIncrementCycle(context);
            }

            if (settings.TryGet<bool?>(KrEditSettingsVirtual.DoNotSkipStage) != true
                && this.TransitFromDifferentGroup(stage, context.MainCardID))
            {
                SetVisibility(false);
                return this.StartApproval(context, StageHandlerResult.SkipResult);
            }

            SetVisibility(true);

            var digest = settings.TryGet<string>(KrEditSettingsVirtual.Comment);
            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);
            if (author == null)
            {
                return StageHandlerResult.EmptyResult;
            }
            var authorID = author.AuthorID;
            var incrementCycle = settings.TryGet<bool?>(KrEditSettingsVirtual.IncrementCycle) == true;

            var sentTask = context.WorkflowAPI.SendTask(
                returnToStage || !incrementCycle
                    ? DefaultTaskTypes.KrEditInterjectTypeID
                    : DefaultTaskTypes.KrEditTypeID,
                string.Empty,
                settings.Get<Guid>(KrSinglePerformerVirtual.PerformerID),
                settings.Get<string>(KrSinglePerformerVirtual.PerformerName),
                modifyTaskAction: p =>
                {
                    p.AuthorID = authorID;
                    p.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                    p.Planned = stage.Planned;
                    p.PlannedQuants = stage.PlannedQuants;
                    p.GroupRowID = groupID;
                    if (!string.IsNullOrWhiteSpace(digest))
                    {
                        p.Digest = digest;
                    }
                    HandlerHelper.SetTaskKind(p, kindID, kindCaption, context);
                }).Task;
            sentTask.Flags |= CardTaskFlags.CreateHistoryItem;
            context.ContextualSatellite.AddToHistory(sentTask.RowID, context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1));
            context.WorkflowAPI.AddActiveTask(sentTask.RowID);

            if (context.CardExtensionContext is ICardStoreExtensionContext)
            {
                context.ValidationResult.Add(
                    this.NotificationManager.SendAsync(
                        DefaultNotifications.TaskNotification,
                        new[] { sentTask.RoleID },
                        new NotificationSendContext()
                        {
                            MainCardID = context.MainCardID ?? Guid.Empty,
                            Info = NotificationHelper.GetInfoWithTask(sentTask),
                            ModifyEmailActionAsync = async (email, ct) =>
                            {
                                NotificationHelper.ModifyEmailForMobileApprovers(
                                    email,
                                    sentTask,
                                    await NotificationHelper.GetMobileApprovalEmailAsync(this.CardCache, ct));

                                NotificationHelper.ModifyTaskCaption(
                                    email,
                                    sentTask);
                            },
                            GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                        }).GetAwaiter().GetResult()); // TODO async
            }

            if (settings.Get<bool>(KrEditSettingsVirtual.ChangeState)
                && !returnToStage
                && !this.KrScope.Info.TryGet<bool>(Keys.IgnoreChangeState))
            {
                context.WorkflowProcess.State = KrState.Editing;
            }

            return StageHandlerResult.InProgressResult;

            void SetVisibility(
                bool visible)
            {
                if (settings.TryGet<bool?>(KrEditSettingsVirtual.ManageStageVisibility) == true)
                {
                    context.Stage.Hidden = !visible;
                }
            }
        }

        public override StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            var task = context.TaskInfo.Task;
            var taskType = task.TypeID;
            if (taskType != DefaultTaskTypes.KrEditTypeID
                 && taskType != DefaultTaskTypes.KrEditInterjectTypeID)
            {
                return StageHandlerResult.EmptyResult;
            }

            if (task.Card.Sections.TryGetValue(KrTaskCommentVirtual.Name, out var commSec)
                && commSec.Fields.TryGetValue(KrTaskCommentVirtual.Comment, out var commentObj)
                && commentObj is string comment)
            {
                context.WorkflowProcess.AuthorComment = comment;

                if (!string.IsNullOrEmpty(comment))
                {
                    HandlerHelper.SetTaskResult(context, task, comment);
                }
            }

            context.WorkflowAPI.RemoveActiveTask(context.TaskInfo.Task.RowID);
            
            HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
            
            return this.StartApproval(context);
        }

        public override bool HandleStageInterrupt(
            IStageTypeHandlerContext context)
        {
            context.Stage.InfoStorage.Remove(ReturnToStage);
            return this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));
        }

        #endregion
    }
}