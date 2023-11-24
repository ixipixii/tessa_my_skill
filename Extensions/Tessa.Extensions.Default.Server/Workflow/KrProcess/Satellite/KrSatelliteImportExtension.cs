using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    /// <summary>
    /// Создание сателлита при импорте карточки.
    /// </summary>
    public sealed class KrSatelliteImportExtension :
        CardSatelliteImportExtension
    {
        #region fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Constructors

        public KrSatelliteImportExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository extendedRepositoryWithoutTransaction,
            IKrTypesCache krTypesCache)
            : base(extendedRepositoryWithoutTransaction)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="CardSatelliteDeleteExtension" and @item="TryGetSatelliteCardAsync"]'/>
        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCard(mainCard, KrConstants.KrSatelliteInfoKey);

        /// <doc path='info[@type="CardSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base);

        #endregion
    }
}
