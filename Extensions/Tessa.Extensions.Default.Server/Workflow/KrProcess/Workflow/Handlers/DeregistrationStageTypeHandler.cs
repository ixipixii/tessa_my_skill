using System;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Numbers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Roles;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class DeregistrationStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public DeregistrationStageTypeHandler(
            INumberDirectorContainer numberDirectorContainer,
            IKrScope krScope,
            ISession session,
            IBusinessCalendarService calendarService,
            ICardMetadata cardMetadata,
            IKrStageSerializer serializer,
            IKrEventManager eventManager)
        {
            this.NumberDirectorContainer = numberDirectorContainer;
            this.KrScope = krScope;
            this.Session = session;
            this.CalendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
            this.CardMetadata = cardMetadata;
            this.Serializer = serializer;
            this.EventManager = eventManager;
        }

        #endregion

        #region Protected Properties

        protected INumberDirectorContainer NumberDirectorContainer { get; set; }

        protected IKrScope KrScope { get; set; }

        protected IBusinessCalendarService CalendarService { get; set; }

        protected ISession Session { get; set; }

        protected ICardMetadata CardMetadata { get; set; }

        protected IKrStageSerializer Serializer { get; set; }

        protected IKrEventManager EventManager { get; set; }

        #endregion

        #region Protected Methods

        protected virtual CardTaskHistoryItem CreateRegistrationTaskHistoryItem(IStageTypeHandlerContext context)
        {
            const string result = "$ApprovalHistory_DocumentDeregistered";
            var optionID = DefaultCompletionOptions.DeregisterDocument;
            var userID = this.Session.User.ID;
            var userName = this.Session.User.Name;
            var utcNow = DateTime.UtcNow;
            var offsetNow = utcNow;

            var groupID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
            // Временная зона текущего сотрудника, для записи в историю заданий
            var userZoneInfo = this.CalendarService.GetRoleTimeZoneInfoAsync(userID).GetAwaiter().GetResult(); // TODO async
            var option = this.CardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[optionID]; // TODO async
            return new CardTaskHistoryItem
            {
                State = CardTaskHistoryState.Inserted,
                RowID = Guid.NewGuid(),
                TypeID = DefaultTaskTypes.KrDeregistrationTypeID,
                TypeName = DefaultTaskTypes.KrDeregistrationTypeName,
                TypeCaption = "$CardTypes_TypesNames_KrDeregistration",
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

        protected virtual int GetCycle(
            IStageTypeHandlerContext context)
        {
            if (context.RunnerMode == KrProcessRunnerMode.Async
                && context.ProcessInfo.ProcessTypeName == KrConstants.KrProcessName)
            {
                // Для основного процесса цикл лежит в его инфо.
                return context.WorkflowProcess.InfoStorage.TryGet<int?>(KrConstants.Keys.Cycle) ?? 1;
            }

            return ProcessInfoCacheHelper.Get(this.Serializer, context.ContextualSatellite)?.TryGet<int?>(KrConstants.Keys.Cycle)
                ?? 0;
        }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
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

                numberDirector.NotifyOnDeregisteringCardAsync(numberContext).GetAwaiter().GetResult(); // TODO async
                context.ValidationResult.Add(numberContext.ValidationResult);

                var info = ProcessInfoCacheHelper.Get(this.Serializer, context.ContextualSatellite);
                var previousState = info?.TryGet<int?>(KrConstants.Keys.StateBeforeRegistration) ?? 0;
                info?.Remove(KrConstants.Keys.StateBeforeRegistration);

                context.WorkflowProcess.State = previousState == KrState.Approved.ID || previousState == KrState.Signed.ID
                    ? (KrState)previousState
                    : KrState.Draft;

                var cycle = this.GetCycle(context);
                var fakeHistoryRecord = this.CreateRegistrationTaskHistoryItem(context);
                mainCard.TaskHistory.Add(fakeHistoryRecord);
                context.ContextualSatellite.AddToHistory(fakeHistoryRecord.RowID, cycle);

                this.EventManager.RaiseAsync(DefaultEventTypes.RegistrationEvent, context).GetAwaiter().GetResult(); // TODO async
            }

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}