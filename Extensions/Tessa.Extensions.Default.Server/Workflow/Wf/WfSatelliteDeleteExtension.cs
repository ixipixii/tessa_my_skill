using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Удаление основной карточки, учитывающее удаление карточки-сателлита Workflow.
    /// </summary>
    public sealed class WfSatelliteDeleteExtension :
        CardSatelliteDeleteExtension
    {
        #region Constructors

        public WfSatelliteDeleteExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository extendedRepositoryWithoutTransaction)
            : base(extendedRepositoryWithoutTransaction)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        protected override Guid SatelliteTypeID => DefaultCardTypes.WfSatelliteTypeID;

        protected override Task<Guid?> TryGetSatelliteIDAsync(IDbScope dbScope, Guid mainCardID, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatelliteIDAsync(dbScope, mainCardID, cancellationToken);

        protected override async ValueTask<bool> SatelliteCardWasNotFoundAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.SatelliteWasNotFound(mainCard);

        protected override async ValueTask<Card> TryGetSatelliteCardAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatellite(mainCard);

        #endregion
    }
}
