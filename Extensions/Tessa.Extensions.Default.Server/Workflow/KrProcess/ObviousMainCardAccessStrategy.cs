using System;
using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class ObviousMainCardAccessStrategy: IMainCardAccessStrategy
    {
        private readonly Card card;

        public ObviousMainCardAccessStrategy(
            Card card)
        {
            this.card = card;
        }

        /// <inheritdoc />
        public bool WasUsed { get; private set; }

        /// <inheritdoc />
        public Card GetCard(
            IValidationResultBuilder validationResult = null,
            bool withoutTransaction = false)
        {
            if (!this.WasUsed)
            {
                this.WasUsed = true;
            }
            return this.card;
        }

        /// <inheritdoc />
        public ICardFileContainer GetFileContainer(
            IValidationResultBuilder validationResult = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void EnsureTaskHistoryLoaded(
            IValidationResultBuilder validationResult = null)
        {
            if (!this.WasUsed)
            {
                this.WasUsed = true;
            }
        }
    }
}