using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class NullMainCardAccessStrategy: IMainCardAccessStrategy
    {
        private NullMainCardAccessStrategy()
        {

        }

        /// <inheritdoc />
        public bool WasUsed => false;

        /// <inheritdoc />
        public Card GetCard(
            IValidationResultBuilder validationResult = null,
            bool withoutTransaction = false)
        {
            return null;
        }

        /// <inheritdoc />
        public ICardFileContainer GetFileContainer(
            IValidationResultBuilder validationResult = null)
        {
            return null;
        }

        /// <inheritdoc />
        public void EnsureTaskHistoryLoaded(
            IValidationResultBuilder validationResult = null)
        {
        }

        public static NullMainCardAccessStrategy Instance = new NullMainCardAccessStrategy();

    }
}