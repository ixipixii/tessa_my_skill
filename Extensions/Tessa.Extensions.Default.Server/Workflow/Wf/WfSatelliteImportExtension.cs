using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Импорт карточки вместе с карточкой-сателлитом Workflow, если она была создана при экспорте.
    /// </summary>
    public sealed class WfSatelliteImportExtension :
        CardSatelliteImportExtension
    {
        #region Constructors

        public WfSatelliteImportExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction)
            : base(extendedRepositoryWithoutTransaction)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="CardSatelliteDeleteExtension" and @item="TryGetSatelliteCardAsync"]'/>
        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatellite(mainCard);

        /// <doc path='info[@type="CardSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        #endregion
    }
}