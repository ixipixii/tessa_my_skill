using System;
using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrTaskHistoryResolver: IKrTaskHistoryResolver
    {
        private readonly IMainCardAccessStrategy cardAccessStrategy;

        private readonly object placeholderContext;

        private readonly IValidationResultBuilder validationResult;

        public KrTaskHistoryResolver(
            IMainCardAccessStrategy cardAccessStrategy,
            object placeholderContext,
            IValidationResultBuilder validationResult,
            ICardTaskHistoryManager taskHistoryManager)
        {
            this.cardAccessStrategy = cardAccessStrategy;
            this.TaskHistoryManager = taskHistoryManager;
            this.validationResult = validationResult;
            this.placeholderContext = placeholderContext;
        }


        /// <inheritdoc />
        public ICardTaskHistoryManager TaskHistoryManager { get; }

        /// <inheritdoc />
        public CardTaskHistoryGroup ResolveTaskHistoryGroup(
            Guid groupTypeID,
            Guid? parentGroupTypeID = null,
            bool newIteration = false,
            IValidationResultBuilder overrideValidationResult = null)
        {
            var card = this.cardAccessStrategy.GetCard(overrideValidationResult);
            this.cardAccessStrategy.EnsureTaskHistoryLoaded(overrideValidationResult);

            return this.TaskHistoryManager.ResolveGroupAsync(
                card,
                card.TaskHistoryGroups,
                card.TaskHistoryGroups,
                overrideValidationResult ?? this.validationResult,
                groupTypeID,
                parentGroupTypeID,
                newIteration,
                this.placeholderContext,
                cardHasNoSections: true).GetAwaiter().GetResult(); // TODO async
        }
    }
}