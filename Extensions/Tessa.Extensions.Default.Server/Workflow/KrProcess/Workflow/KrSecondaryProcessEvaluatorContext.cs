using System;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrSecondaryProcessEvaluatorContext: IKrSecondaryProcessEvaluatorContext
    {
        public KrSecondaryProcessEvaluatorContext(
            IKrSecondaryProcess secondaryProcess,
            IValidationResultBuilder validationResult,
            IMainCardAccessStrategy mainCardAccessStrategy,
            Guid? cardID,
            Guid? cardTypeID,
            string cardTypeName,
            string cardTypeCaption,
            Guid? docTypeID,
            KrComponents? krComponents,
            KrState? state,
            Card contextualSatellite,
            ICardExtensionContext cardContext)
        {
            this.SecondaryProcess = secondaryProcess;
            this.ValidationResult = validationResult;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.CardID = cardID;
            this.CardTypeID = cardTypeID;
            this.CardTypeName = cardTypeName;
            this.CardTypeCaption = cardTypeCaption;
            this.DocTypeID = docTypeID;
            this.KrComponents = krComponents;
            this.State = state;
            this.ContextualSatellite = contextualSatellite;
            this.CardContext = cardContext;
        }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess { get; }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        /// <inheritdoc />
        public IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <inheritdoc />
        public Guid? CardID { get; }

        /// <inheritdoc />
        public Guid? CardTypeID { get; }

        /// <inheritdoc />
        public string CardTypeName { get; }

        /// <inheritdoc />
        public string CardTypeCaption { get; }

        /// <inheritdoc />
        public Guid? DocTypeID { get; }

        /// <inheritdoc />
        public KrComponents? KrComponents { get; }

        /// <inheritdoc />
        public KrState? State { get; }

        /// <inheritdoc />
        public Card ContextualSatellite { get; }

        /// <inheritdoc />
        public ICardExtensionContext CardContext { get; }
    }
}