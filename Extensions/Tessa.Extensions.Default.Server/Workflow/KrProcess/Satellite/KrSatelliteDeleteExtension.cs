using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    /// <summary>
    /// Удаление карточки-сателлита при удалении основной карточки.
    /// </summary>
    public sealed class KrSatelliteDeleteExtension : CardSatelliteDeleteExtension
    {
        #region fields

        private readonly IKrTypesCache krCache;

        #endregion

        #region Constructors

        public KrSatelliteDeleteExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository extendedRepositoryWithoutTransaction,
            IKrTypesCache krCache)
            : base(extendedRepositoryWithoutTransaction)
        {
            this.krCache = krCache;
        }

        #endregion

        #region Base Overrides

        protected override Guid SatelliteTypeID => DefaultCardTypes.KrSatelliteTypeID;

        protected override Task<Guid?> TryGetSatelliteIDAsync(IDbScope dbScope, Guid mainCardID, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteIDAsync(
                dbScope,
                mainCardID,
                KrConstants.KrApprovalCommonInfo.Name,
                KrConstants.KrProcessCommonInfo.MainCardID,
                cancellationToken);

        protected override async ValueTask<bool> SatelliteCardWasNotFoundAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.SatelliteCardWasNotFound(mainCard, KrConstants.KrSatelliteInfoKey);

        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCard(mainCard, KrConstants.KrSatelliteInfoKey);

        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krCache).Has(KrComponents.Base);

        #endregion
    }
}
