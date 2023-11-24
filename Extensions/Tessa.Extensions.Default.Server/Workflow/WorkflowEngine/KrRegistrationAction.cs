using System.Threading.Tasks;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Cards.Numbers;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.WorkflowEngine;
using Tessa.Workflow;
using Tessa.Workflow.Compilation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.WorkflowEngine
{
    public sealed class KrRegistrationAction : KrWorkflowActionBase
    {
        #region Fields

        private readonly INumberDirectorContainer numberDirectorContainer;

        #endregion

        #region Constructors

        public KrRegistrationAction(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository cardRepository,
            IWorkflowEngineCardRequestExtender requestExtender,
            INumberDirectorContainer numberDirectorContainer,
            IBusinessCalendarService calendarService)
            :base(KrDescriptors.RegistrationDescriptor, cardRepository, requestExtender, calendarService)
        {
            this.numberDirectorContainer = numberDirectorContainer;
        }

        #endregion

        #region Base Overrides

        protected override async Task ExecuteAsync(IWorkflowEngineContext context, IWorkflowEngineCompiled scriptObject)
        {
            var mainCard = await context.GetMainCardAsync(context.CancellationToken);
            if (mainCard == null)
            {
                return;
            }

            var cardType = (await context.CardMetadata.GetCardTypesAsync(context.CancellationToken))[mainCard.TypeID];

            // выделение номера при регистрации
            var numberProvider = this.numberDirectorContainer.GetProvider(cardType.ID);
            var numberDirector = numberProvider.GetDirector();
            var numberComposer = numberProvider.GetComposer();
            var numberContext = await numberDirector.CreateContextAsync(
                numberComposer,
                mainCard,
                cardType,
                transactionMode: NumberTransactionMode.WithoutTransaction,
                cancellationToken: context.CancellationToken);

            await numberDirector.NotifyOnRegisteringCardAsync(numberContext, context.CancellationToken);
            context.ValidationResult.Add(numberContext.ValidationResult);

            if (context.ValidationResult.IsSuccessful())
            {
                await AddTaskHistoryByTaskAsync(
                    context,
                    DefaultTaskTypes.KrRegistrationTypeID,
                    DefaultCompletionOptions.RegisterDocument,
                    "$ApprovalHistory_DocumentRegistered");

                await SetStateIDAsync(
                    context,
                    KrState.Registered.ID,
                    KrState.Registered.TryGetDefaultName());
            }
        }

        #endregion
    }
}
