using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ResolutionStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public ResolutionStageTypeHandler(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository cardRepository,
            ICardMetadata cardMetadata,
            ICardGetStrategy cardGetStrategy,
            IKrScope krScope,
            IKrTokenProvider tokenProvider,
            IRoleRepository roleRepository,
            ISession session)
        {
            this.CardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
            this.CardMetadata = cardMetadata ?? throw new ArgumentNullException(nameof(cardMetadata));
            this.CardGetStrategy = cardGetStrategy ?? throw new ArgumentNullException(nameof(cardGetStrategy));
            this.KrScope = krScope ?? throw new ArgumentNullException(nameof(krScope));
            this.TokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            this.RoleRepository = roleRepository;
            this.Session = session;
        }

        #endregion

        #region Protected Properties and Constants

        protected const string TaskRowID = nameof(TaskRowID);

        protected ICardRepository CardRepository { get; set; }
        protected ICardMetadata CardMetadata { get; set; }
        protected ICardGetStrategy CardGetStrategy { get; set; }
        protected IKrScope KrScope { get; set; }
        protected IKrTokenProvider TokenProvider { get; set; }
        protected IRoleRepository RoleRepository { get; set; }
        protected ISession Session { get; set; }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var performers = context.Stage.Performers;
            if (performers.Count == 0)
            {
                return StageHandlerResult.CompleteResult;
            }

            if (!(context.CardExtensionContext is ICardStoreExtensionContext storeContext))
            {
                return StageHandlerResult.EmptyResult;
            }
            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return StageHandlerResult.EmptyResult;
            }
            var authorID = author.AuthorID;
            var authorName = author.AuthorName;
            var task = new CardTask() { TypeID = DefaultTaskTypes.WfResolutionProjectTypeID };
            var settings = context.Stage.SettingsStorage;
            var resolutionFields = task.Card.Sections.GetOrAddEntry(WfHelper.ResolutionSection).Fields;
            resolutionFields[WfHelper.ResolutionKindIDField] = settings[KrResolutionSettingsVirtual.KindID];
            resolutionFields[WfHelper.ResolutionKindCaptionField] = settings[KrResolutionSettingsVirtual.KindCaption];
            resolutionFields[WfHelper.ResolutionAuthorIDField] = authorID;
            resolutionFields[WfHelper.ResolutionAuthorNameField] = authorName;
            resolutionFields[WfHelper.ResolutionControllerIDField] = settings[KrResolutionSettingsVirtual.ControllerID];
            resolutionFields[WfHelper.ResolutionControllerNameField] = settings[KrResolutionSettingsVirtual.ControllerName];
            resolutionFields[WfHelper.ResolutionCommentField] = settings[KrResolutionSettingsVirtual.Comment];
            resolutionFields[WfHelper.ResolutionPlannedField] = settings[KrResolutionSettingsVirtual.Planned];
            resolutionFields[WfHelper.ResolutionDurationInDaysField] = settings[KrResolutionSettingsVirtual.DurationInDays];
            resolutionFields[WfHelper.ResolutionWithControlField] = settings[KrResolutionSettingsVirtual.WithControl];
            resolutionFields[WfHelper.ResolutionMassCreationField] = settings[KrResolutionSettingsVirtual.MassCreation];
            resolutionFields[WfHelper.ResolutionMajorPerformerField] = settings[KrResolutionSettingsVirtual.MajorPerformer];

            var performerIndex = 0;
            var performerRows = task.Card.Sections.GetOrAddTable(WfHelper.ResolutionPerformersSection).Rows;
            foreach (var performer in performers)
            {
                var performerRow = performerRows.Add();
                performerRow.RowID = Guid.NewGuid();
                performerRow.State = CardRowState.Inserted;
                performerRow[WfHelper.ResolutionPerformerRoleIDField] = performer.PerformerID;
                performerRow[WfHelper.ResolutionPerformerRoleNameField] = performer.PerformerName;
                performerRow[WfHelper.ResolutionPerformerOrderField] = performerIndex++;
            }

            var card = storeContext.Request.Card.Clone();
            var token = this.TokenProvider.CreateToken(card);

            card.Sections.Clear();
            card.Files.Clear();
            card.Tasks.Clear();
            card.TaskHistory.Clear();
            card.Info.Clear();
            token.Set(card.Info);

            var historyGroupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var rowID = Guid.NewGuid();
            var response = this.CardRepository.StoreAsync(new CardStoreRequest { Card = card }
                .SetStartingProcessTaskGroupRowID(historyGroupID)
                .SetStartingProcessNextTask(task)
                .SetStartingProcessName(WfHelper.ResolutionProcessName)
                .SetStartingProcessTaskRowID(rowID)).GetAwaiter().GetResult(); // TODO async

            context.ValidationResult.Add(response.ValidationResult);
            if (!response.ValidationResult.IsSuccessful())
            {
                return StageHandlerResult.EmptyResult;
            }

            var process = context.ProcessInfo;
            var scope = context.CardExtensionContext.DbScope;
            var db = scope.Db;

            db
                .SetCommand(
                    scope.BuilderFactory
                        .Update("TaskHistory")
                        .C("ProcessID").Assign().P("ProcessID")
                        .C("ProcessKind").Assign().P("ProcessKind")
                        .Where().C("RowID").Equals().P("RowID")
                        .Build(),
                    db.Parameter("RowID", rowID),
                    db.Parameter("ProcessID", process.ProcessID),
                    db.Parameter("ProcessKind", process.ProcessTypeName))
                .ExecuteNonQuery();

            context.Stage.InfoStorage[TaskRowID] = rowID;
            return StageHandlerResult.InProgressResult;
        }


        public override bool HandleStageInterrupt(IStageTypeHandlerContext context)
        {
            var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
            var scope = storeContext.DbScope;
            using (scope.Create())
            {
                var db = scope.Db;
                var tasksToRevoke = db
                    .SetCommand(
                        scope.BuilderFactory
                            .With("ChildTaskHistory", e => e
                                    .Select().C("t", "RowID")
                                    .From("TaskHistory", "t").NoLock()
                                    .Where().C("t", "ParentRowID").Equals().P("RootRowID")
                                    .UnionAll()
                                    .Select().C("t", "RowID")
                                    .From("TaskHistory", "t").NoLock()
                                    .InnerJoin("ChildTaskHistory", "p")
                                    .On().C("p", "RowID").Equals().C("t", "ParentRowID"),
                                columnNames: new[] { "RowID" },
                                recursive: true)
                            .Select().C("t", "RowID")
                            .From("Tasks", "t").NoLock()
                            .InnerJoin("ChildTaskHistory", "h")
                            .On().C("h", "RowID").Equals().C("t", "RowID")
                            .Build(),
                        db.Parameter("RootRowID", context.Stage.InfoStorage.Get<Guid>(TaskRowID)))
                    .LogCommand()
                    .ExecuteList<Guid>();
                if (tasksToRevoke.Count == 0)
                {
                    return true;
                }

                var tasksToLoad = tasksToRevoke;
                var card = this.KrScope.GetMainCard(storeContext.Request.Card.ID);
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
                    var taskContexts = this.CardGetStrategy.TryLoadTaskInstancesAsync(
                        card.ID, card, db, this.CardMetadata, context.ValidationResult, storeContext.Session.User.ID,
                        getTaskMode: CardGetTaskMode.All, loadCalendarInfo: false, taskRowIDList: tasksToLoad).GetAwaiter().GetResult(); // TODO async;
                    foreach (var taskContext in taskContexts)
                    {
                        this.CardGetStrategy.LoadSectionsAsync(taskContext).GetAwaiter().GetResult(); // TODO async
                    }
                }

                cardTasks = card.TryGetTasks();
                if (cardTasks == null)
                {
                    return true;
                }

                foreach (var taskToRevoke in cardTasks)
                {
                    if (tasksToRevoke.Contains(taskToRevoke.RowID))
                    {
                        taskToRevoke.Action = CardTaskAction.Complete;
                        taskToRevoke.State = CardRowState.Deleted;
                        taskToRevoke.Flags = taskToRevoke.Flags & ~CardTaskFlags.Locked | CardTaskFlags.UnlockedForAuthor | CardTaskFlags.HistoryItemCreated;
                        taskToRevoke.Result = "$ApprovalHistory_TaskCancelledBacauseCancelling";
                        taskToRevoke.OptionID = DefaultCompletionOptions.Cancel;

                        var resolutionFields = taskToRevoke.Card.Sections.GetOrAddEntry(WfHelper.ResolutionSection).Fields;
                        resolutionFields[WfHelper.ResolutionRevokeChildrenField] = BooleanBoxes.False;
                    }
                }

                // Всегда true, поскольку задачи обитают в другом процессе.
                return true;
            }
        }


        public override StageHandlerResult HandleSignal(IStageTypeHandlerContext context)
        {
            var signal = context.SignalInfo;
            if (signal?.Signal?.Name == KrPerformSignal)
            {
                return StageHandlerResult.CompleteResult;
            }

            return StageHandlerResult.EmptyResult;
        }

        #endregion
    }
}
