using System;
using System.Threading;
using Tessa.Cards;
using Tessa.Platform.Validation;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class KrStageTypeUIHandlerContext : IKrStageTypeUIHandlerContext
    {
        #region Fields

        private readonly GridRowAction? action;

        #endregion

        #region Constructors

        public KrStageTypeUIHandlerContext(
            Guid stageTypeID,
            GridRowAction? action,
            GridViewModel control,
            CardRow row,
            ICardModel rowModel,
            ICardModel cardModel,
            IValidationResultBuilder validationResult,
            CancellationToken cancellationToken = default)
        {
            this.StageTypeID = stageTypeID;
            this.action = action;
            this.Control = control;
            this.Row = row;
            this.RowModel = rowModel;
            this.CardModel = cardModel;
            this.ValidationResult = validationResult;
            this.CancellationToken = cancellationToken;
        }

        #endregion

        #region IExtensionContext Members

        /// <doc path='info[@type="IExtensionContext" and @item="CancellationToken"]'/>
        public CancellationToken CancellationToken { get; set; }

        #endregion

        #region IKrStageTypeUIHandlerContext Members

        /// <inheritdoc />
        public Guid StageTypeID { get; }

        /// <inheritdoc />
        public GridRowAction Action =>
            this.action ?? throw new InvalidOperationException("GridRowAction doesn't exist in current context.");

        /// <inheritdoc />
        public GridViewModel Control { get; }

        /// <inheritdoc />
        public CardRow Row { get; }

        /// <inheritdoc />
        public ICardModel RowModel { get; }

        /// <inheritdoc />
        public ICardModel CardModel { get; }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        #endregion
    }
}