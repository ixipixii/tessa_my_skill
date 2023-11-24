using System;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrSecondaryProcessEvaluatorContext
    {
        /// <summary>
        /// Кнпока, для которой проводится вычисление.
        /// </summary>
        IKrSecondaryProcess SecondaryProcess { get; }

        /// <summary>
        /// Результат валидации.
        /// </summary>
        IValidationResultBuilder ValidationResult { get; }

        /// <summary>
        /// Стратегия загрузки основной карточки.
        /// </summary>
        IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <summary>
        /// Идентификатор карточки.
        /// </summary>
        Guid? CardID { get; }
        /// <summary>
        /// Идентификатор типа карточки.
        /// </summary>
        Guid? CardTypeID { get; }

        /// <summary>
        /// Название типа карточки.
        /// </summary>
        string CardTypeName { get; }

        /// <summary>
        /// Отображаемое название типа карточки.
        /// </summary>
        string CardTypeCaption { get; }

        /// <summary>
        /// Идентификатор типа документа.
        /// </summary>
        Guid? DocTypeID { get; }

        /// <summary>
        /// Включенные компоненты типового решения для текущей карточки.
        /// </summary>
        KrComponents? KrComponents { get; }

        /// <summary>
        /// Состояние карточки.
        /// </summary>
        KrState? State { get; }

        /// <summary>
        /// Контекстуальный сателлит карточки.
        /// </summary>
        Card ContextualSatellite { get; }

        /// <summary>
        /// Конекст расширения карточки.
        /// </summary>
        ICardExtensionContext CardContext { get; }
    }
}