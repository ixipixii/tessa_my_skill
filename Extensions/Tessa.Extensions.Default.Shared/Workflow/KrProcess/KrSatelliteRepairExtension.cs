using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    /// <summary>
    /// Расширение, вызывающее исправление с расширениями для карточки-сателлита KrSatellite.
    /// </summary>
    public sealed class KrSatelliteRepairExtension :
        CardSatelliteRepairExtension
    {
        #region Constructors

        public KrSatelliteRepairExtension(IKrTypesCache krTypesCache) =>
            this.krTypesCache = krTypesCache;

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="CardSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            cardType.Flags.HasNot(CardTypeFlags.Hidden) // другой сателлит
            && KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache) != KrComponents.None;

        /// <doc path='info[@type="CardSatelliteDeleteExtension" and @item="TryGetSatelliteCardAsync"]'/>
        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCard(mainCard, KrConstants.KrSatelliteInfoKey);

        #endregion
    }
}
