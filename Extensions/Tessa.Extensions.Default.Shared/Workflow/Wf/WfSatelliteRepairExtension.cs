using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Shared.Workflow.Wf
{
    /// <summary>
    /// Расширение, вызывающее исправление с расширениями для карточки-сателлита WfSatellite.
    /// </summary>
    public sealed class WfSatelliteRepairExtension :
        CardSatelliteRepairExtension
    {
        #region Constructors

        public WfSatelliteRepairExtension(IKrTypesCache krTypesCache) =>
            this.krTypesCache = krTypesCache;

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="CardSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        /// <doc path='info[@type="CardSatelliteDeleteExtension" and @item="TryGetSatelliteCardAsync"]'/>
        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCard(mainCard, WfHelper.SatelliteKey);

        #endregion
    }
}
