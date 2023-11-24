using System;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessButtonVisibilityEvaluatorContext : IKrProcessButtonVisibilityEvaluatorContext
    {
        #region Constructors

        public KrProcessButtonVisibilityEvaluatorContext(
            IValidationResultBuilder validationResult,
            IMainCardAccessStrategy mainCardAccessStrategy,
            Guid? cardID,
            Guid? cardTypeID,
            string cardTypeName,
            string cardTypeCaption,
            Guid? docTypeID,
            KrComponents? krComponents,
            KrState? state,
            ICardExtensionContext cardContext)
        {
            this.ValidationResult = validationResult;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.CardID = cardID;
            this.CardTypeID = cardTypeID;
            this.CardTypeName = cardTypeName;
            this.CardTypeCaption = cardTypeCaption;
            this.DocTypeID = docTypeID;
            this.KrComponents = krComponents;
            this.State = state;
            this.CardContext = cardContext;
        }

        public KrProcessButtonVisibilityEvaluatorContext(
            IValidationResultBuilder validationResult)
        {
            this.ValidationResult = validationResult;
            this.MainCardAccessStrategy = null;
            this.CardID = null;
            this.CardTypeID = null;
            this.CardTypeName = null;
            this.CardTypeCaption = null;
            this.DocTypeID = null;
            this.KrComponents = null;
            this.State = null;
            this.CardContext = null;
        }

        public KrProcessButtonVisibilityEvaluatorContext(
            IValidationResultBuilder validationResult,
            ObviousMainCardAccessStrategy mainCardAccessStrategy,
            KrComponents? krComponents,
            Guid? docTypeID,
            KrState? state,
            ICardExtensionContext cardContext)
        {
            this.ValidationResult = validationResult;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.CardID = mainCardAccessStrategy.GetCard().ID;
            this.CardTypeID = mainCardAccessStrategy.GetCard().TypeID;
            this.CardTypeName = mainCardAccessStrategy.GetCard().TypeName;
            this.CardTypeCaption = mainCardAccessStrategy.GetCard().TypeCaption;
            this.DocTypeID = docTypeID;
            this.KrComponents = krComponents;
            this.State = state;
            this.CardContext = cardContext;
        }

        #endregion

        #region IKrProcessButtonVisibilityEvaluatorContext Members

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
        public ICardExtensionContext CardContext { get; }

        #endregion
    }
}