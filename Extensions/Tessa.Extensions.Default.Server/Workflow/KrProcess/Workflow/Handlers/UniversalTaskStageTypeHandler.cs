using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;
using NotificationHelper = Tessa.Extensions.Default.Shared.Notices.NotificationHelper;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class UniversalTaskStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public UniversalTaskStageTypeHandler(
            IKrScope krScope,
            IRoleRepository roleRepository,
            ISession session,
            IBusinessCalendarService calendarService,
            IStageTasksRevoker tasksRevoker,
            [Dependency(NotificationManagerNames.DeferredWithoutTransaction)] INotificationManager notificationManager,
            ICardCache cardCache)
        {
            this.KrScope = krScope;
            this.RoleRepository = roleRepository;
            this.Session = session;
            this.CalendarService = calendarService;
            this.TasksRevoker = tasksRevoker;
            this.NotificationManager = notificationManager;
            this.CardCache = cardCache;
        }

        #endregion

        #region Protected Properties and Constants

        protected IKrScope KrScope { get; }

        protected IRoleRepository RoleRepository { get; }

        protected ISession Session { get; }

        protected IBusinessCalendarService CalendarService { get; }

        protected IStageTasksRevoker TasksRevoker { get; }

        protected INotificationManager NotificationManager { get; }

        protected ICardCache CardCache { get; }

        protected const string TotalTasksCountKey = CardHelper.SystemKeyPrefix + "TotalTasksCount";

        protected const string CompletedTasksCountKey = CardHelper.SystemKeyPrefix + "CompletedTasksCount";

        protected const string TasksKey = Keys.Tasks;

        #endregion

        #region Protected Methods

        protected virtual StageHandlerResult SendUniversalTask(IStageTypeHandlerContext context)
        {
            var performers = context.Stage.Performers;
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);

            if (kindID is null)
            {
                // TODO: fallback, удалить позже. На смену пришла стандартная настройка, получаемая через  HandlerHelper.GetTaskKind
                kindID = context.Stage.SettingsStorage.TryGet<Guid?>(key: KrUniversalTaskSettingsVirtual.KindID);
                kindCaption = context.Stage.SettingsStorage.TryGet<string>(KrUniversalTaskSettingsVirtual.KindCaption);
            }
            
            context.Stage.InfoStorage[CompletedTasksCountKey] = Int32Boxes.Zero;
            context.Stage.InfoStorage[TotalTasksCountKey] = Int32Boxes.Box(performers.Count);
            context.Stage.InfoStorage[TasksKey] = null;

            if (performers.Count == 0)
            {
                return StageHandlerResult.CompleteResult;
            }

            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return StageHandlerResult.EmptyResult;
            }
            var authorID = author.AuthorID;
            var stageOptionsRows = context.Stage.SettingsStorage.TryGet<List<object>>(KrUniversalTaskOptionsSettingsVirtual.Synthetic);

            if (stageOptionsRows == null
                || stageOptionsRows.Count == 0)
            {
                context.ValidationResult.AddError(this, "$KrProcess_UniversalTask_NoCompletionOptions");
                return StageHandlerResult.EmptyResult;
            }

            foreach (var performer in performers)
            {
                var api = context.WorkflowAPI;
                var taskInfo = api.SendTask(
                    DefaultTaskTypes.KrUniversalTaskTypeID,
                    context.Stage.SettingsStorage.Get<string>(KrUniversalTaskSettingsVirtual.Digest),
                    performer.PerformerID,
                    performer.PerformerName,
                    modifyTaskAction: t =>
                    {
                        t.AuthorID = authorID;
                        t.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                        t.GroupRowID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                        t.Planned = context.Stage.Planned;
                        t.PlannedQuants = context.Stage.PlannedQuants;
                        HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                    });

                api.AddActiveTask(taskInfo.Task.RowID);
                var task = taskInfo.Task;
                task.Flags |= CardTaskFlags.CreateHistoryItem;
                context.ContextualSatellite.AddToHistory(task.RowID, context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1));


                var optionsSection = task.Card.Sections.GetOrAddTable(nameof(KrUniversalTaskOptions));

                foreach (Dictionary<string, object> row in stageOptionsRows)
                {
                    var newRow = optionsSection.Rows.Add();
                    newRow.RowID = Guid.NewGuid();
                    newRow[KrUniversalTaskOptions.OptionID] = row.TryGet<Guid>(KrUniversalTaskOptionsSettingsVirtual.OptionID);
                    newRow[KrUniversalTaskOptions.Caption] = row.TryGet<string>(KrUniversalTaskOptionsSettingsVirtual.Caption);
                    newRow[KrUniversalTaskOptions.ShowComment] = row.TryGet<bool>(KrUniversalTaskOptionsSettingsVirtual.ShowComment);
                    newRow[KrUniversalTaskOptions.Additional] = row.TryGet<bool>(KrUniversalTaskOptionsSettingsVirtual.Additional);
                    newRow[KrUniversalTaskOptions.Order] = row.TryGet<int?>(KrUniversalTaskOptionsSettingsVirtual.Order);
                    newRow[KrUniversalTaskOptions.Message] = row.TryGet<string>(KrUniversalTaskOptionsSettingsVirtual.Message);
                    newRow.State = CardRowState.Inserted;
                }

                context.ValidationResult.Add(
                    this.NotificationManager.SendAsync(
                        DefaultNotifications.TaskNotification,
                        new Guid[] { task.RoleID },
                        new NotificationSendContext()
                        {
                            MainCardID = context.MainCardID ?? Guid.Empty,
                            Info = NotificationHelper.GetInfoWithTask(task),
                            ModifyEmailActionAsync = async (email, ct) =>
                            {
                                NotificationHelper.ModifyEmailForMobileApprovers(
                                    email,
                                    task,
                                    await NotificationHelper.GetMobileApprovalEmailAsync(CardCache, ct));

                                NotificationHelper.ModifyTaskCaption(
                                    email,
                                    task);
                            },

                            GetCardFuncAsync = (ct) => new ValueTask<Card>(context.MainCardAccessStrategy.GetCard()),
                        }).GetAwaiter().GetResult()); // TODO async
            }

            return StageHandlerResult.InProgressResult;
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
            return this.SendUniversalTask(context);
        }

        public override StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            var totalCount = context.Stage.InfoStorage.TryGet<int>(TotalTasksCountKey);
            var completedCount = context.Stage.InfoStorage.TryGet<int>(CompletedTasksCountKey) + 1;
            var taskInfo = context.TaskInfo;
            var optionID = taskInfo.Task.Info.TryGet<Guid>(KrUniversalTaskStoreExtension.OptionIDKey);
            var comment = taskInfo.Task.Card.Sections.GetOrAdd(nameof(KrTask)).RawFields.TryGet<string>(KrTask.Comment);
            var optionRow = taskInfo
                 .Task
                 .Card
                 .Sections
                 .GetOrAddTable(nameof(KrUniversalTaskOptions))
                 .Rows
                 .FirstOrDefault(x => x.Get<Guid?>(KrUniversalTaskOptions.OptionID) == optionID);

            if (optionRow == null)
            {
                context.ValidationResult.AddError(this, $"Invalid completion option: {optionID:B}.");
                return StageHandlerResult.EmptyResult;
            }

            var taskStorage = StorageHelper.Clone(taskInfo.Task.GetStorage());
            var task = new CardTask(taskStorage)
            {
                OptionID = optionID
            };
            task.RemoveChanges();

            if (!context.Stage.WriteTaskFullInformation)
            {
                taskStorage.Remove(nameof(CardTask.SectionRows));
                taskStorage.Remove(nameof(CardTask.Card));
                taskStorage.Remove(CardInfoStorageObject.InfoKey);
            }

            taskStorage["Comment"] = comment;
            taskStorage["OptionName"] = optionRow.Get<string>(KrUniversalTaskOptions.Caption);
            taskStorage["CompletedByID"] = this.Session.User.ID;
            taskStorage["CompletedByName"] = this.Session.User.Name;
            taskStorage["Completed"] = context.CardExtensionContext is CardStoreExtensionContext storeContext ? storeContext.StoreDateTime : DateTime.UtcNow;

            context.Stage.InfoStorage[CompletedTasksCountKey] = completedCount;
            HandlerHelper.AppendToCompletedTasks(context.Stage, task);

            return totalCount <= completedCount
                ? StageHandlerResult.CompleteResult
                : StageHandlerResult.InProgressResult;
        }

        public override bool HandleStageInterrupt(IStageTypeHandlerContext context) =>
            this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));

        #endregion
    }
}