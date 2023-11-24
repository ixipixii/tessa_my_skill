using System;
using System.Collections.Generic;
using System.Threading;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public sealed class KrStageRowExtensionContext : IKrStageRowExtensionContext
    {
        #region Constructors

        public KrStageRowExtensionContext(
            IDictionary<Guid, IDictionary<string, object>> stageStorages,
            Card cardToRepair,
            Card innerCard,
            Card outerCard,
            CardType cardType,
            RouteCardType routeCardType,
            ICardExtensionContext cardContext,
            CancellationToken cancellationToken = default)
        {
            this.StageStorages = stageStorages;
            this.CardToRepair = cardToRepair;
            this.InnerCard = innerCard;
            this.OuterCard = outerCard;
            this.CardType = cardType;
            this.RouteCardType = routeCardType;
            this.CardContext = cardContext;
            this.CancellationToken = cancellationToken;
        }

        #endregion

        #region IExtensionContext Members

        /// <doc path='info[@type="IExtensionContext" and @item="CancellationToken"]'/>
        public CancellationToken CancellationToken { get; set; }

        #endregion

        #region IKrStageRowExtensionContext Members

        /// <inheritdoc />
        public IDictionary<Guid, IDictionary<string, object>> StageStorages { get; }

        /// <inheritdoc />
        public Card CardToRepair { get; }

        /// <inheritdoc />
        public Card InnerCard { get; }

        /// <inheritdoc />
        public Card OuterCard { get; }

        /// <inheritdoc />
        public CardType CardType { get; }

        /// <inheritdoc />
        public RouteCardType RouteCardType { get; }

        /// <inheritdoc />
        public ICardExtensionContext CardContext { get; }

        #endregion
    }
}