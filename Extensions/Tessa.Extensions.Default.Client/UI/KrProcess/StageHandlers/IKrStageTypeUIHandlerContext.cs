using System;
using Tessa.Cards;
using Tessa.Platform.Validation;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public interface IKrStageTypeUIHandlerContext :
        IExtensionContext
    {
        /// <summary>
        /// ID типа этапа.
        /// </summary>
        Guid StageTypeID { get; }

        /// <summary>
        /// Действие со строкой <see cref="Row"/>.
        /// </summary>
        GridRowAction Action { get; }

        /// <summary>
        /// Контрол, в рамках которого выполняется событие.
        /// </summary>
        GridViewModel Control { get; }

        /// <summary>
        /// Строка карточки, с которой производится действие.
        /// </summary>
        CardRow Row { get; }

        /// <summary>
        /// Модель строки <see cref="Row"/> вместе с формой, которая только что была инициализирована,
        /// или <c>null</c>, если выполняется удаление строки, при этом модель не требуется.
        /// </summary>
        ICardModel RowModel { get; }

        /// <summary>
        /// Модель карточки, в которой расположена строка <see cref="Row"/>.
        /// </summary>
        ICardModel CardModel { get; }

        /// <summary>
        /// Результат валидации. Актуально только для события валидации строки.
        /// </summary>
        IValidationResultBuilder ValidationResult { get; }

    }
}