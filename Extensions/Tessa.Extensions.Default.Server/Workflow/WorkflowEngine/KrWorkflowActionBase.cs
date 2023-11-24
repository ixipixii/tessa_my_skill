using System;
using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Tessa.Workflow;
using Tessa.Workflow.Actions;
using Tessa.Workflow.Actions.Descriptors;
using Tessa.Workflow.Helpful;

namespace Tessa.Extensions.Default.Server.Workflow.WorkflowEngine
{
    public abstract class KrWorkflowActionBase : WorkflowActionBase
    {
        #region Fields And Consts

        protected const string AffectMainCardVersionWhenStateChangedKey = "AffectMainCardVersionWhenStateChanged";
        protected const string PreviousStateKey = "KrPreviousState";

        protected readonly ICardRepository cardRepository;
        protected readonly IWorkflowEngineCardRequestExtender requestExtender;
        protected readonly IBusinessCalendarService calendarService;

        #endregion

        #region Constructors

        protected KrWorkflowActionBase(
            WorkflowActionDescriptor actionDescriptor,
            ICardRepository cardRepository,
            IWorkflowEngineCardRequestExtender requestExtender,
            IBusinessCalendarService calendarService)
            :base(actionDescriptor)
        {
            this.cardRepository = cardRepository;
            this.requestExtender = requestExtender;
            this.calendarService = calendarService;
        }

        #endregion

        #region Protected Methods

        protected async Task<ValidationResult> SetStateIDAsync(
            IWorkflowEngineContext context,
            int stateID,
            string stateName)
        {
            var mainCardID = context.ProcessInstance.CardID;
            CardGetRequest getRequest = new CardGetRequest
            {
                CardID = mainCardID,
                CardTypeID = DefaultCardTypes.KrSatelliteTypeID,
                RestrictionFlags = CardGetRestrictionValues.Satellite,
            };
            requestExtender.ExtendGetRequest(getRequest);
            getRequest.SetForbidStoringHistory(true);

            CardGetResponse response = await cardRepository.GetAsync(getRequest, context.CancellationToken);

            if (!response.ValidationResult.IsSuccessful())
            {
                return response.ValidationResult.Build();
            }

            var sCard = response.Card;

            var oldStateID = sCard.Sections[KrConstants.KrApprovalCommonInfo.Name].RawFields.TryGet<int>(KrConstants.StateID);
            if (oldStateID == stateID)
            {
                // Если состояния равны, то смену состояния не производим
                return ValidationResult.Empty;
            }

            StorePreviousState(context, oldStateID);
            sCard.Sections[KrConstants.KrApprovalCommonInfo.Name].Fields[KrConstants.StateID] = stateID;
            sCard.Sections[KrConstants.KrApprovalCommonInfo.Name].Fields[KrConstants.StateName] = stateName;
            sCard.Sections[KrConstants.KrApprovalCommonInfo.Name].Fields[KrConstants.KrApprovalCommonInfo.StateChangedDateTimeUTC] = DateTime.UtcNow;

            var storeRequest = new CardStoreRequest { Card = sCard };
            requestExtender.ExtendStoreRequest(storeRequest);
            var storeResponse = await this.cardRepository.StoreAsync(storeRequest, context.CancellationToken);

            var mainCard = await context.GetMainCardAsync(context.CancellationToken);
            mainCard.Sections[KrConstants.KrApprovalCommonInfo.Virtual].Fields[KrConstants.StateID] = stateID;
            mainCard.Sections[KrConstants.KrApprovalCommonInfo.Virtual].Fields[KrConstants.StateName] = stateName;

            if (await context.GetAsync<bool?>(AffectMainCardVersionWhenStateChangedKey) ?? true)
            {
                context.ModifyStoreRequest(ModifyRequest);
            }

            return storeResponse.ValidationResult.Build();
        }

        protected async Task AddTaskHistoryByTaskAsync(
            IWorkflowEngineContext context,
            Guid taskTypeID,
            Guid optionID,
            string result)
        {
            if (!(await context.CardMetadata.GetCardTypesAsync(context.CancellationToken)).TryGetValue(taskTypeID, out var taskType))
            {
                return;
            }

            await AddTaskHistoryAsync(
                context,
                taskTypeID,
                taskType.Name,
                taskType.Caption,
                optionID,
                result);
        }

        protected async Task AddTaskHistoryAsync(
            IWorkflowEngineContext context,
            Guid taskTypeID,
            string taskTypeName,
            string taskTypeCaption,
            Guid optionID,
            string result)
        {
            var userID = context.Session.User.ID;
            var userName = context.Session.User.Name;

            var groupID = context.ProcessInstance.GetHistoryGroup();
            // Временная зона текущего сотрудника, для записи в историю заданий
            var userZoneInfo = await this.calendarService.GetRoleTimeZoneInfoAsync(userID, context.CancellationToken);
            var option = (await context.CardMetadata.GetEnumerationsAsync(context.CancellationToken)).CompletionOptions[optionID];
            var newItem = new CardTaskHistoryItem
            {
                State = CardTaskHistoryState.Inserted,
                RowID = Guid.NewGuid(),
                TypeID = taskTypeID,
                TypeName = taskTypeName,
                TypeCaption = taskTypeCaption,
                Created = context.StoreDateTime,
                Planned = context.StoreDateTime,
                InProgress = context.StoreDateTime,
                Completed = context.StoreDateTime,
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

            var mainCard = await context.GetMainCardAsync(context.CancellationToken);
            if (mainCard != null)
            {
                mainCard.TaskHistory.Add(newItem);
            }
        }

        protected void StorePreviousState(IWorkflowEngineContext context, int previousState)
        {
            context.ProcessInstance.Hash[PreviousStateKey] = previousState;
        }

        protected int TryGetPreviousState(IWorkflowEngineContext context)
        {
            return context.ProcessInstance.Hash.TryGet<int?>(PreviousStateKey) ?? KrState.Draft.ID;
        }

        #endregion

        #region Private Methods

        private void ModifyRequest(CardStoreRequest request)
        {
            request.AffectVersion = true;
        }

        #endregion
    }
}
