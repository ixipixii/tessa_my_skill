using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public interface IKrStageRowExtensionContext :
        IExtensionContext
    {
        /// <summary>
        /// Настройки этапов.
        /// </summary>
        IDictionary<Guid, IDictionary<string, object>> StageStorages { get; }

        /// <summary>
        /// Карточка с настройками этапа, используемая для восстановления при загрузке.
        /// </summary>
        Card CardToRepair { get; }

        /// <summary>
        /// "Входная" карточка.
        /// </summary>
        Card InnerCard { get; }

        /// <summary>
        /// "Выходная" карточка.
        /// </summary>
        Card OuterCard { get; }

        /// <summary>
        /// Тип карточки.
        /// </summary>
        CardType CardType { get; }

        /// <summary>
        /// Тип карточки маршрута.
        /// </summary>
        RouteCardType RouteCardType { get; }

        /// <summary>
        /// Внешний контекст расширения. Может быть <c>null</c>.
        /// </summary>
        ICardExtensionContext CardContext { get; }
    }
}