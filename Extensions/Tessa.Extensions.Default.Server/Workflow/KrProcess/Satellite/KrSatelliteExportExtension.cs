using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public class KrSatelliteExportExtension: CardSatelliteExportExtension
    {
        private readonly IKrTypesCache typesCache;

        /// <inheritdoc />
        public KrSatelliteExportExtension(
            ICardRepository cardRepository,
            ICardTransactionStrategy cardTransactionStrategy,
            IKrTypesCache typesCache)
            : base(cardRepository, cardTransactionStrategy)
        {
            this.typesCache = typesCache;
        }

        /// <inheritdoc />
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.typesCache).Has(KrComponents.Base);

        /// <inheritdoc />
        protected override Task<Guid?> TryGetSatelliteIDAsync(IDbScope dbScope, Guid mainCardID, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteIDAsync(
                dbScope,
                mainCardID,
                KrConstants.KrApprovalCommonInfo.Name,
                KrConstants.KrProcessCommonInfo.MainCardID,
                cancellationToken);

        protected override async ValueTask SetSatelliteAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.SetSatellite(mainCard, satellite, KrConstants.KrSatelliteInfoKey);
    }
}