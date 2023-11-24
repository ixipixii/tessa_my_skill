using System;
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
    /// Восстановление карточки-сателлита Workflow при восстановлении основной карточки.
    /// </summary>
    public sealed class WfSatelliteRestoreExtension :
        CardSatelliteRestoreExtension
    {
        #region Constructors

        public WfSatelliteRestoreExtension(
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
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        protected override ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            new ValueTask<Card>(WfHelper.TryGetSatellite(mainCard));

        #endregion
    }
}