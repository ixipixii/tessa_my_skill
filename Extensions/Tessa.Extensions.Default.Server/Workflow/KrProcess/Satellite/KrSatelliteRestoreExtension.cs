using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    /// <summary>
    /// Восстановление карточки-сателлита при восстановлении основной карточки.
    /// </summary>
    public sealed class KrSatelliteRestoreExtension :
        CardSatelliteRestoreExtension
    {
        #region Constructors

        public KrSatelliteRestoreExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository extendedRepositoryWithoutTransaction,
            IKrTypesCache krTypesCache)
            : base(extendedRepositoryWithoutTransaction) =>
            this.krTypesCache = krTypesCache ?? throw new ArgumentNullException(nameof(krTypesCache));

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            new ValueTask<bool>(KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base));

        protected override ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            new ValueTask<Card>(CardSatelliteHelper.TryGetSatelliteCard(mainCard, KrConstants.KrSatelliteInfoKey));

        #endregion
    }
}