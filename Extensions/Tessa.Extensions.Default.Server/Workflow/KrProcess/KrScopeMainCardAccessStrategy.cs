using System;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrScopeMainCardAccessStrategy: IMainCardAccessStrategy
    {
        private readonly Guid cardID;

        private readonly IKrScope scope;

        private readonly IValidationResultBuilder result;

        public KrScopeMainCardAccessStrategy(
            Guid cardID,
            IKrScope scope,
            IValidationResultBuilder result = null)
        {
            this.cardID = cardID;
            this.scope = scope;
            this.result = result;
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
            return this.scope.GetMainCard(this.cardID, validationResult ?? this.result, withoutTransaction: true);
        }

        /// <inheritdoc />
        public ICardFileContainer GetFileContainer(
            IValidationResultBuilder validationResult = null)
        {
            if (!this.WasUsed)
            {
                this.WasUsed = true;
            }
            return this.scope.GetMainCardFileContainer(this.cardID, validationResult ?? this.result);
        }

        /// <inheritdoc />
        public void EnsureTaskHistoryLoaded(
            IValidationResultBuilder validationResult = null)
        {
            if (!this.WasUsed)
            {
                this.WasUsed = true;
            }
            this.scope.EnsureMainCardHasTaskHistory(this.cardID, validationResult ?? this.result);
        }
    }
}