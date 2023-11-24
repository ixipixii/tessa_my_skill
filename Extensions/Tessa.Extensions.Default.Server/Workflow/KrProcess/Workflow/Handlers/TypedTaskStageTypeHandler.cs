using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Notices;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Properties.Resharper;
using Tessa.Roles;
using Unity;
using NotificationHelper = Tessa.Extensions.Default.Shared.Notices.NotificationHelper;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class TypedTaskStageTypeHandler : StageTypeHandlerBase
    {
        #region nested types

        public class ScriptContext
        {
            private readonly Func<Guid, Performer, DateTime?, double?, string, CardTask> sendTaskAction;

            private readonly IStageTypeHandlerContext context;


            private readonly IStageTasksRevoker tasksRevoker;

            public ScriptContext(
                CardTask task,
                IStageTypeHandlerContext context,
                IStageTasksRevoker tasksRevoker,
                Func<Guid, Performer, DateTime?, double?, string, CardTask> sendTaskAction)
            {
                this.Task = task;
                this.sendTaskAction = sendTaskAction;
                this.context = context;
                this.tasksRevoker = tasksRevoker;
            }

            public CardTask Task { [UsedImplicitly] get; }

            [UsedImplicitly]
            public bool CompleteStage { get; set; } = false;

            [UsedImplicitly]
            public CardTask SendTask(
                Performer performer,
                Guid? taskType = null,
                DateTime? planned = null,
                double? timeLimit = null,
                string digest = null)
            {
                var actualTaskType = taskType
                    ?? this.context.Stage.SettingsStorage.TryGet<Guid>(KrConstants.KrTypedTaskSettingsVirtual.TaskTypeID);
                var actualDigest = digest
                    ?? this.context.Stage.SettingsStorage.TryGet<string>(KrConstants.KrTypedTaskSettingsVirtual.TaskDigest);

                return this.sendTaskAction(actualTaskType, performer, planned, timeLimit, actualDigest);
            }

            [UsedImplicitly]
            public int RevokeTask(
                Guid taskID)
            {
                var cardID = this.context.MainCardID;
                if (cardID is null)
                {
                    throw new NullReferenceException("MainCardID");
                }

                return this.tasksRevoker.RevokeTask(new StageTaskRevokerContext(this.context)
                {
                    CardID = cardID.Value,
                    TaskID = taskID,
                })
                    ? 1
                    : 0;
            }

            [UsedImplicitly]
            public int RevokeTask(
                IEnumerable<Guid> taskIDs)
            {
                var cardID = this.context.MainCardID;
                if (cardID is null)
                {
                    throw new NullReferenceException("MainCardID");
                }

                return this.tasksRevoker.RevokeTasks(new StageTaskRevokerContext(this.context)
                {
                    CardID = cardID.Value,
                    TaskIDs = taskIDs.ToList(),
                });
            }
        }

        #endregion

        #region fields

        public static readonly string ScriptContextParameterType =
            $"global::{typeof(TypedTaskStageTypeHandler).FullName}.{typeof(ScriptContext).Name}";

        public const string AfterTaskMethodName = "AfterTask";

        public const string MethodParameterName = "TypedTaskContext";

        protected const string ActiveTasksCount = nameof(ActiveTasksCount);

        protected const string CompleteStageCountdown = nameof(CompleteStageCountdown);

        protected readonly IKrScope KrScope;

        protected readonly ISession Session;

        protected readonly IRoleRepository RoleRepository;

        protected readonly IKrCompilationCache CompilationCache;

        protected readonly IUnityContainer UnityContainer;

        protected readonly IStageTasksRevoker TasksRevoker;

        protected readonly IDbScope DbScope;

        protected readonly INotificationManager NotificationManager;

        protected readonly ICardCache CardCache;

        #endregion

        #region constructor

        public TypedTaskStageTypeHandler(
            IBusinessCalendarService calendarService,
            IKrScope krScope,
            ISession session,
            IRoleRepository roleRepository,
            IKrCompilationCache compilationCache,
            IUnityContainer unityContainer,
            IStageTasksRevoker tasksRevoker,
            IDbScope dbScope,
            [Dependency(NotificationManagerNames.DeferredWithoutTransaction)] INotificationManager notificationManager,
            ICardCache cardCache)
        {
            this.KrScope = krScope;
            this.Session = session;
            this.RoleRepository = roleRepository;
            this.CompilationCache = compilationCache;
            this.UnityContainer = unityContainer;
            this.TasksRevoker = tasksRevoker;
            this.DbScope = dbScope;
            this.NotificationManager = notificationManager;
            this.CardCache = cardCache;
        }

        #endregion

        #region Base overrides

        /// <inheritdoc />
        public override void BeforeInitialization(IStageTypeHandlerContext context)
        {
            HandlerHelper.ClearCompletedTasks(context.Stage);
        }

        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            var performers = stage.Performers;
            if (performers.Count == 0)
            {
                return StageHandlerResult.CompleteResult;
            }

            var taskType = context.Stage.SettingsStorage.TryGet<Guid>(KrConstants.KrTypedTaskSettingsVirtual.TaskTypeID);
            var digest = context.Stage.SettingsStorage.TryGet<string>(KrConstants.KrTypedTaskSettingsVirtual.TaskDigest);

            foreach (var performer in performers)
            {
                this.SendTask(context, taskType, performer, null, null, digest);
            }

            stage.InfoStorage.Remove(CompleteStageCountdown);
            stage.InfoStorage[ActiveTasksCount] = performers.Count;

            return StageHandlerResult.InProgressResult;
        }

        /// <inheritdoc />
        public override StageHandlerResult HandleTaskCompletion(
            IStageTypeHandlerContext context)
        {
            var task = context.TaskInfo.Task;
            var stage = context.Stage;

            var completeStageCountdown = stage.InfoStorage.TryGet<int>(CompleteStageCountdown);
            if (completeStageCountdown >= 1)
            {
                stage.InfoStorage[CompleteStageCountdown] = --completeStageCountdown;
                return completeStageCountdown == 0
                    ? StageHandlerResult.CompleteResult
                    : StageHandlerResult.InProgressResult;
            }

            if (task.State == CardRowState.Deleted)
            {
                stage.InfoStorage[ActiveTasksCount] = stage.InfoStorage.TryGet<int>(ActiveTasksCount) - 1;

                var taskStorage = StorageHelper.Clone(task.GetStorage());
                var taskCopy = new CardTask(taskStorage);
                taskCopy.RemoveChanges();

                taskStorage.Remove(nameof(CardTask.SectionRows));
                taskStorage.Remove(nameof(CardTask.Card));
                taskStorage.Remove(CardInfoStorageObject.InfoKey);

                HandlerHelper.AppendToCompletedTasks(stage, taskCopy);
            }

            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);
            var ctx = new ScriptContext (
                task,
                context,
                this.TasksRevoker,
                (ttid, prf, plnd, tmlmt, dg) =>
                {
                    stage.InfoStorage[ActiveTasksCount] = stage.InfoStorage.TryGet<int>(ActiveTasksCount) + 1;
                    return this.SendTask(context, ttid, prf, plnd, tmlmt, dg);
                });
            inst.InvokeExtra(AfterTaskMethodName, ctx);

            if (ctx.CompleteStage)
            {
                return this.CompleteStage(context);
            }

            return stage.InfoStorage.TryGet<int>(ActiveTasksCount) == 0
                ? StageHandlerResult.CompleteResult
                : StageHandlerResult.InProgressResult;
        }

        public override bool HandleStageInterrupt(IStageTypeHandlerContext context) =>
            this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));

        #endregion

        #region protected

        protected CardTask SendTask(
            IStageTypeHandlerContext context,
            Guid taskType,
            Performer performer,
            DateTime? planned,
            double? timeLimit,
            string digest)
        {
            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return null;
            }
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);

            var authorID = author.AuthorID;
            var task = context.WorkflowAPI.SendTask(
                taskType,
                digest,
                performer.PerformerID,
                performer.PerformerName,
                modifyTaskAction: t =>
                {
                    t.AuthorID = authorID;
                    t.AuthorName = null; // AuthorName и AuthorPosition определяются системой, когда явно указано null
                    t.GroupRowID = groupID;
                    t.Planned = planned ?? context.Stage.Planned;
                    t.PlannedQuants =
                        timeLimit.HasValue
                            ? (int)Math.Round(timeLimit.Value * TimeZonesHelper.QuantsInDay)
                            : context.Stage.PlannedQuants;
                    HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                }).Task;
            task.Flags |= CardTaskFlags.CreateHistoryItem;
            context.ContextualSatellite.AddToHistory(task.RowID,
                context.WorkflowProcess.InfoStorage.TryGet(KrConstants.Keys.Cycle, 1));

            context.WorkflowAPI.AddActiveTask(task.RowID);
            context.ValidationResult.Add(
                this.NotificationManager.SendAsync(
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
                                await NotificationHelper.GetMobileApprovalEmailAsync(this.CardCache, ct));

                            NotificationHelper.ModifyTaskCaption(
                                email,
                                task);
                        },
                        GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                    }).GetAwaiter().GetResult()); // TODO async

            return task;
        }

        protected StageHandlerResult CompleteStage(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            using (this.DbScope.Create())
            {
                var db = this.DbScope.Db;
                // Получение списка заданий из таблицы WorkflowTasks
                var currentTasks = db.SetCommand(
                        this.DbScope.BuilderFactory
                            .Select()
                            .C("RowID")
                            .From("WorkflowTasks").NoLock()
                            .Where().C("ProcessRowID").Equals().P("pid")
                            .Build(),
                        db.Parameter("pid", context.ProcessInfo.ProcessID))
                    .LogCommand()
                    .ExecuteList<Guid>();
                if (currentTasks.Count == 0)
                {
                    return StageHandlerResult.CompleteResult;
                }
                stage.InfoStorage[CompleteStageCountdown] = currentTasks.Count;
                this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));
                return StageHandlerResult.InProgressResult;
            }
        }

        #endregion
    }
}