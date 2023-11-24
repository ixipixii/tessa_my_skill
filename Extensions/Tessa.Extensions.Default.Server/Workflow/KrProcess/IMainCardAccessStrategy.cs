using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public interface IMainCardAccessStrategy
    {
        /// <summary>
        /// Признак того, что стратегия использовалась, т.е. вызывались методы доступа к карточке.
        /// </summary>
        bool WasUsed { get; }

        /// <summary>
        /// Получение объекта карточки в соответствии с правилами стратегии.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <param name="withoutTransaction"></param>
        /// <returns></returns>
        Card GetCard(
            IValidationResultBuilder validationResult = null,
            bool withoutTransaction = false);

        /// <summary>
        /// Получение файлового контейнера карточки в соответствии с правилами стратегии.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        ICardFileContainer GetFileContainer(
            IValidationResultBuilder validationResult = null);
        
        /// <summary>
        /// Загрузка (при необходимости) истории заданий в карточку в соответствии с правилами стратегии.
        /// </summary>
        /// <param name="validationResult"></param>
        void EnsureTaskHistoryLoaded(
            IValidationResultBuilder validationResult = null);
    }
}