using System;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Numbers;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class RegistrationStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public RegistrationStageTypeHandler(
            INumberDirectorContainer numberDirectorContainer,
            IKrScope krScope,
            ISession session,
            ICardMetadata cardMetadata,
            IKrStageSerializer serializer,
            IKrEventManager eventManager,
            IBusinessCalendarService calendarService,
            IRoleRepository roleRepository,
            IStageTasksRevoker tasksRevoker)
        {
            this.NumberDirectorContainer = numberDirectorContainer;
            this.KrScope = krScope;
            this.Session = session;
            this.CardMetadata = cardMetadata;
            this.Serializer = serializer;
            this.EventManager = eventManager;
            this.CalendarService = calendarService;
            this.RoleRepository = roleRepository;
            this.TasksRevoker = tasksRevoker;
        }

        #endregion

        #region Protected Properties

        protected INumberDirectorContainer NumberDirectorContainer { get; set; }

        protected IKrScope KrScope { get; set; }

        protected ISession Session { get; set; }

        protected ICardMetadata CardMetadata { get; set; }

        protected IKrStageSerializer Serializer { get; set; }

        protected IKrEventManager EventManager { get; set; }

        protected IBusinessCalendarService CalendarService { get; set; }

        protected IRoleRepository RoleRepository { get; set; }

        protected IStageTasksRevoker TasksRevoker { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Непосредственная регистрация прямо сейчас.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="taskInfo">Регистрация производится после задания</param>
        /// <returns></returns>
        protected virtual StageHandlerResult SyncRegistration(
            IStageTypeHandlerContext context,
            IWorkflowTaskInfo taskInfo = null)
        {
            // Непосредственная регистрация карточки.
            if (context.MainCardID.HasValue
                && context.MainCardTypeID.HasValue)
            {
                var cardType = this.CardMetadata.GetCardTypesAsync().GetAwaiter().GetResult()[context.MainCardTypeID.Value]; // TODO async
                var mainCard = this.KrScope.GetMainCard(context.MainCardID.Value);

                // выделение номера при регистрации
                var numberProvider = this.NumberDirectorContainer.GetProvider(context.MainCardTypeID);
                var numberDirector = numberProvider.GetDirector();
                var numberComposer = numberProvider.GetComposer();
                var numberContext = numberDirector.CreateContextAsync(
                    numberComposer,
                    mainCard,
                    cardType,
                    context.CardExtensionContext is ICardStoreExtensionContext storeContext
                        ? storeContext.Request.Info
                        : null,
                    context.CardExtensionContext,
                    transactionMode: NumberTransactionMode.WithoutTransaction).GetAwaiter().GetResult(); // TODO async

                numberDirector.NotifyOnRegisteringCardAsync(numberContext).GetAwaiter().GetResult(); // TODO async
                context.ValidationResult.Add(numberContext.ValidationResult);

                this.EventManager.RaiseAsync(DefaultEventTypes.RegistrationEvent, context).GetAwaiter().GetResult(); // TODO async

                var cycle = this.GetCycle(context);
                if (taskInfo != null)
                {
                    context.ContextualSatellite.AddToHistory(taskInfo.Task.RowID, cycle);
                }
                else
                {
                    var fakeHistoryRecord = this.CreateRegistrationTaskHistoryItem(context);
                    mainCard.TaskHistory.Add(fakeHistoryRecord);
                    context.ContextualSatellite.AddToHistory(fakeHistoryRecord.RowID, cycle);
                }

            }
            context.WorkflowProcess.State = KrState.Registered;
            return StageHandlerResult.CompleteResult;
        }


        protected virtual StageHandlerResult AsyncRegistration(
            IStageTypeHandlerContext context)
        {
            var api = context.WorkflowAPI;

            // Получаем исполнителя, указанного в настройках этапа.
            var performer = context.Stage.Performer;
            if (performer is null)
            {
                context.ValidationResult.AddError(this, "$KrStages_Registration_PerformerNotSpecified");
                return StageHandlerResult.EmptyResult;
            }
            var performerID = performer.PerformerID;
            var performerName = performer.PerformerName;

            // Установка в карточке состояния "На регистрации"
            context.WorkflowProcess.State = KrState.Registration;

            var digest = context.Stage.SettingsStorage.TryGet<string>(KrRegistrationStageSettingsVirtual.Comment)
                ?? context.Stage.Name;

            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);

            var author = HandlerHelper.GetStageAuthor(context, this.RoleRepository, this.Session);
            if (author == null)
            {
                return StageHandlerResult.EmptyResult;
            }
            var authorID = author.AuthorID;
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);
            // Отправка задания регистрации
            var taskInfo = api.SendTask(
                DefaultTaskTypes.KrRegistrationTypeID,
                digest,
                performerID,
                performerName,
                modifyTaskAction: t =>
                {
                    t.AuthorID = authorID;
                    t.AuthorName = null;    // AuthorName и AuthorPosition определяются системой, когда явно указано null
                    t.Planned = context.Stage.Planned;
                    t.PlannedQuants = context.Stage.PlannedQuants;
                    t.GroupRowID = groupID;
                    t.Flags |= CardTaskFlags.CreateHistoryItem;
                    HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                });
            // Добавление задания в список активных заданий,
            // которые будут отображатся в таблице над заданиями.
            api.AddActiveTask(taskInfo.Task.RowID);

            // Результат говорит подсистеме маршрутов о том, что этап находится в процессе выполнения
            return StageHandlerResult.InProgressResult;
        }


        protected virtual CardTaskHistoryItem CreateRegistrationTaskHistoryItem(IStageTypeHandlerContext context)
        {
            if (!this.CardMetadata.GetCardTypesAsync().GetAwaiter().GetResult() // TODO async
                .TryGetValue(DefaultTaskTypes.KrRegistrationTypeID, out var taskType))
            {
                return null;
            }

            const string result = "$ApprovalHistory_DocumentRegistered";
            var optionID = DefaultCompletionOptions.RegisterDocument;
            var userID = this.Session.User.ID;
            var userName = this.Session.User.Name;
            var utcNow = DateTime.UtcNow;

            // чтобы регистрация была позже, чем отзыв согласований и другие строчки в NextRequest-е
            var offsetNow = utcNow.AddMilliseconds(500.0);
            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            // Временная зона текущего сотрудника, для записи в историю заданий
            var userZoneInfo = this.CalendarService.GetRoleTimeZoneInfoAsync(userID).GetAwaiter().GetResult(); // TODO async
            var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[optionID]; // TODO async
            return new CardTaskHistoryItem
            {
                State = CardTaskHistoryState.Inserted,
                RowID = Guid.NewGuid(),
                TypeID = DefaultTaskTypes.KrRegistrationTypeID,
                TypeName = taskType.Name,
                TypeCaption = taskType.Caption,
                Created = offsetNow,
                Planned = offsetNow,
                InProgress = offsetNow,
                Completed = offsetNow,
                AuthorID = userID,
                UserID = userID,
                RoleID = userID,
                AuthorName = userName,
                UserName = userName,
                RoleName = userName,
                RoleTypeID = RoleHelper.PersonalRoleTypeID,
                Result = result,
                OptionID = optionID,
                OptionCaption = option.Caption,
                OptionName = option.Name,
                ParentRowID = null,
                CompletedByID = userID,
                CompletedByName = userName,
                GroupRowID = groupID,
                TimeZoneID = userZoneInfo.TimeZoneID,
                TimeZoneUtcOffsetMinutes = (int?)userZoneInfo.TimeZoneUtcOffset.TotalMinutes
            };
        }


        protected virtual int GetCycle(IStageTypeHandlerContext context)
        {
            if (context.RunnerMode == KrProcessRunnerMode.Async
                && context.ProcessInfo.ProcessTypeName == KrProcessName)
            {
                // Для основного процесса цикл лежит в его инфо.
                return context.WorkflowProcess.InfoStorage.TryGet<int?>(Keys.Cycle) ?? 1;
            }

            return ProcessInfoCacheHelper.Get(this.Serializer, context.ContextualSatellite)?.TryGet<int?>(Keys.Cycle)
                ?? 0;
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
            // Запоминаем состояние до начала регистрации.
            var info = ProcessInfoCacheHelper.Get(this.Serializer, context.ContextualSatellite);
            info[Keys.StateBeforeRegistration] = (int)context.WorkflowProcess.State;

            // При запуске этапа определяем, в каком режиме сейчас идет выполнение
            switch (context.RunnerMode)
            {
                case KrProcessRunnerMode.Sync:
                    // Выполнение в синхронном режиме, отправка заданий запрещена
                    // Выполняем регистрацию
                    return this.SyncRegistration(context);
                case KrProcessRunnerMode.Async:
                    var withoutTask =
                        context.Stage.SettingsStorage.TryGet<bool?>(KrRegistrationStageSettingsVirtual.WithoutTask);
                    return withoutTask == true
                        // Выполняем регистрацию
                        ? this.SyncRegistration(context)
                        // Выполнение в асинхронном режиме, отправляем задание регистрации
                        : this.AsyncRegistration(context);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            // Завершение задания регистрации
            // Вся информация о задании доступна в контексте
            var taskInfo = context.TaskInfo;
            var task = taskInfo.Task;
            var taskType = task.TypeID;
            var optionID = task.OptionID ?? Guid.Empty;

            if (taskType == DefaultTaskTypes.KrRegistrationTypeID
                && optionID == DefaultCompletionOptions.RegisterDocument)
            {
                // Записываем в список активных заданий
                HandlerHelper.AppendToCompletedTasksWithPreparing(context.Stage, task);

                // Вариант завершения "Зарегистрировать"
                // Удаляем задание из списка активных
                context.WorkflowAPI.TryRemoveActiveTask(taskInfo.Task.RowID);
                // Проводим регистрацию документа
                this.SyncRegistration(context, taskInfo);
                // Сообщаем подсистеме маршрутов о том, что работа этапа завершена.
                return StageHandlerResult.CompleteResult;
            }

            throw new InvalidOperationException();
        }

        public override bool HandleStageInterrupt(IStageTypeHandlerContext context) =>
            this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));

        #endregion
    }
}