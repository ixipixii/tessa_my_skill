using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Экспорт карточки вместе с карточкой-сателлитом Workflow, если она была создана.
    /// </summary>
    public sealed class WfSatelliteBackupExtension :
        CardSatelliteBackupExtension
    {
        #region Constructors

        public WfSatelliteBackupExtension(
            IKrTypesCache krTypesCache,
            ICardRepository cardRepository)
            : base(cardRepository)
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

        protected override Task<Guid?> TryGetSatelliteIDAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatelliteIDAsync(dbScope, mainCardID, cancellationToken);

        protected override async ValueTask SetSatelliteAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            WfHelper.SetSatellite(mainCard, satellite);

        #endregion
    }
}