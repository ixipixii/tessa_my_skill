using System;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class SigningStageTypeHandler : SubtaskStageTypeHandler
    {
        private const string TotalPerformerCount = nameof(TotalPerformerCount);
        private const string CurrentPerformerCount = nameof(CurrentPerformerCount);
        private const string Disapproved = nameof(Disapproved);

        public SigningStageTypeHandler(
            IRoleRepository roleRepository,
            ICardMetadata cardMetadata,
            ICardGetStrategy cardGetStrategy,
            ICardCache cardCache,
            IKrScope krScope,
            IBusinessCalendarService calendarService,
            ISession session,
            IStageTasksRevoker tasksRevoker,
            [Dependency(NotificationManagerNames.DeferredWithoutTransaction)] INotificationManager notificationManager)
            : base(roleRepository, calendarService, cardMetadata, cardGetStrategy, krScope, tasksRevoker, notificationManager, cardCache, session)
        {
            this.CardCache = cardCache ?? throw new ArgumentNullException(nameof(cardCache));
        }

        #region Protected Properties

        protected ICardCache CardCache { get; set; }

        #endregion

        #region Protected Methods

        protected virtual StageHandlerResult CompleteSigningTask(IStageTypeHandlerContext context, bool declined)
        {
            var task = context.TaskInfo.Task;
            HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);
            
            this.RevokeSubTasks(context, task,
                new[] { DefaultTaskTypes.KrRequestCommentTypeID },
                t => t.Result = "$ApprovalHistory_ParentTaskIsCompleted");

            var field = declined ? KrApprovalCommonInfo.DisapprovedBy : KrApprovalCommonInfo.ApprovedBy;
            var fields = context.ContextualSatellite.Sections[KrApprovalCommonInfo.Name].Fields;
            var result = StringBuilderHelper.Acquire();

            var value = fields.TryGet<string>(field);
            if (!string.IsNullOrEmpty(value))
            {
                result.Append(value).Append("; ");
            }

            var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
            var user = storeContext.Session.User;
            result.Append(user.Name);

            if (task.RoleID != user.ID)
            {
                result.Append(" (");

                var role = KrAdditionalApprovalMarker.Unmark(task.RoleName);
                result.Append(role);

                result.Append(')');
            }

            fields[field] = result.ToStringAndRelease();

            var stage = context.Stage;
            var total = stage.InfoStorage.Get<int>(TotalPerformerCount);
            var current = stage.InfoStorage.Get<int>(CurrentPerformerCount);
            stage.InfoStorage[CurrentPerformerCount] = Int32Boxes.Box(++current);

            if (declined)
            {
                stage.InfoStorage[Disapproved] = BooleanBoxes.True;
            }
            if (declined
                || !this.CardCache.Cards
                    .GetAsync(KrSettings.Name).GetAwaiter().GetResult() // TODO async
                    .Sections[KrSettings.Name].RawFields.Get<bool>(KrSettings.HideCommentForApprove))
            {
                HandlerHelper.SetTaskResult(context, task, task.Card.Sections[nameof(KrTask)].Fields.TryGet<string>(nameof(KrTask.Comment)));
            }

            var returnWhenDeclined = stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ReturnWhenDeclined);
            if (current == total)
            {
                return stage.InfoStorage.TryGet<bool>(Disapproved)
                    ? DeclineAndComplete()
                    : SignAndComplete();
            }

            if (!stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.IsParallel))
            {
                if (declined && returnWhenDeclined)
                {
                    return DeclineAndComplete();
                }
                
                var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
                if (author == null)
                {
                    return StageHandlerResult.EmptyResult;
                }
                var authorID = author.AuthorID;
                
                var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);
                this.SendTask(
                    context,
                    DefaultTaskTypes.KrSigningTypeID,
                    this.GetTaskDigest(context),
                    stage.Performers[current],
                    t =>
                    {
                        t.AuthorID = authorID;
                        t.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                        t.GroupRowID = groupID;
                        t.Planned = stage.Planned;
                        t.PlannedQuants = stage.PlannedQuants;
                        HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                    });
            }

            return StageHandlerResult.InProgressResult;

            StageHandlerResult SignAndComplete()
            {
                StagePositionInGroup(out var hasPreviouslyDisapproved, out var hasNext);
                var returnToAuthor = stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ReturnToAuthor);
                
                if (stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ChangeStateOnEnd))
                {
                    if (hasPreviouslyDisapproved)
                    {
                        context.WorkflowProcess.State = KrState.Declined;
                        this.KrScope.Info[Keys.IgnoreChangeState] = BooleanBoxes.True;
                    }
                    else if (!returnToAuthor)
                    {
                        context.WorkflowProcess.State =  KrState.Signed;
                    }
                }

                if (hasPreviouslyDisapproved
                    && !hasNext)
                {
                    // Последний этап завершен. Этот подписан, но предыдущие могли быть и не подписаны
                    // Если были неподписанные, то возвращаемся в начало на доработку
                    return StageHandlerResult.CurrentGroupTransition();
                }

                if (returnToAuthor)
                {
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


                return this.StageCompleted(context, StageHandlerResult.CompleteResult);
            }

            StageHandlerResult DeclineAndComplete()
            {
                StagePositionInGroup(out _, out var hasNext);

                if (!returnWhenDeclined && hasNext)
                {
                    context.WorkflowProcess.InfoStorage[Disapproved] = BooleanBoxes.True;
                    return this.StageCompleted(context, StageHandlerResult.CompleteResult);
                }

                var utcNow = DateTime.UtcNow;
                var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[DefaultCompletionOptions.RebuildDocument]; // TODO async
                var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                // Временная зона текущего сотрудника, для записи в историю заданий
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
                    TimeZoneUtcOffsetMinutes = (int?)userZoneInfo.TimeZoneUtcOffset.TotalMinutes
                };

                this.KrScope.GetMainCard(storeContext.Request.Card.ID).TaskHistory.Add(item);
                context.ContextualSatellite.AddToHistory(item.RowID, context.WorkflowProcess.InfoStorage.TryGet(Keys.Cycle, 1));

                if (context.Stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Declined;
                    this.KrScope.Info[Keys.IgnoreChangeState] = BooleanBoxes.True;
                }

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
                            && currStage.StageTypeID == StageTypeDescriptors.SigningDescriptor.ID
                            && currStage.InfoStorage.TryGet<bool?>(Disapproved) == true)
                        {
                            hasPreviouslyDisapprovedClosure = true;
                        }

                        if (!hasNextClosure
                            && equator
                            && currStage.StageTypeID == StageTypeDescriptors.SigningDescriptor.ID)
                        {
                            hasNextClosure = true;
                        }
                    });
                hasPreviouslyDisapproved = hasPreviouslyDisapprovedClosure;
                hasNext = hasNextClosure;
            }
        }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            if (IsInterjected(context))
            {
                if (stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Signed;
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
                if (stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ChangeStateOnEnd))
                {
                    context.WorkflowProcess.State = KrState.Signed;
                }

                return StageHandlerResult.CompleteResult;
            }

            if (stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.ChangeStateOnStart))
            {
                context.WorkflowProcess.State = KrState.Signing;
            }

            stage.InfoStorage[TotalPerformerCount] = Int32Boxes.Box(performers.Count);
            stage.InfoStorage[CurrentPerformerCount] = Int32Boxes.Zero;
            stage.InfoStorage[Disapproved] = BooleanBoxes.False;

            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return StageHandlerResult.EmptyResult;
            }
            var authorID = author.AuthorID;
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);
            if (stage.SettingsStorage.Get<bool>(KrSigningStageSettingsVirtual.IsParallel))
            {
                foreach (var performer in performers)
                {
                    this.SendTask(
                        context,
                        DefaultTaskTypes.KrSigningTypeID,
                        this.GetTaskDigest(context),
                        performer,
                        t =>
                        {
                            t.AuthorID = authorID;
                            t.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                            t.GroupRowID = groupID;
                            t.Planned = stage.Planned;
                            t.PlannedQuants = stage.PlannedQuants;
                            HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                        });
                }
            }
            else
            {
                this.SendTask(
                    context,
                    DefaultTaskTypes.KrSigningTypeID,
                    this.GetTaskDigest(context),
                    performers[0],
                    t =>
                    {
                        t.AuthorID = authorID;
                        t.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                        t.GroupRowID = groupID;
                        t.Planned = stage.Planned;
                        t.PlannedQuants = stage.PlannedQuants;
                        HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                    });
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
            context.WorkflowAPI.RemoveActiveTask(task.RowID);

            var optionID = task.OptionID;
            if (optionID == DefaultCompletionOptions.Sign)
            {
                return this.CompleteSigningTask(context, false);
            }

            if (optionID == DefaultCompletionOptions.Decline)
            {
                return this.CompleteSigningTask(context, true);
            }

            return StageHandlerResult.EmptyResult;
        }


        public override bool HandleStageInterrupt(IStageTypeHandlerContext context)
            => this.HandleStageInterrupt(context,
                new[]
                {
                    DefaultTaskTypes.KrSigningTypeID,
                    DefaultTaskTypes.KrRequestCommentTypeID
                },
                t => t.Result = "$ApprovalHistory_TaskCancelledBacauseCancelling");

        #endregion
    }
}
