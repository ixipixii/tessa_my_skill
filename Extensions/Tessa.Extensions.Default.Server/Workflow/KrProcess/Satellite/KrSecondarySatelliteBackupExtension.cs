using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrSecondarySatelliteBackupExtension : TaskSatelliteBackupExtension
    {
        private readonly IKrTypesCache typesCache;

        public KrSecondarySatelliteBackupExtension(
            ICardRepository cardRepository,
            IKrTypesCache typesCache)
            : base(cardRepository)
        {
            this.typesCache = typesCache;
        }

        /// <doc path='info[@type="TaskSatelliteBackupExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.KrSecondarySatelliteTypeID;

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.typesCache);

        /// <doc path='info[@type="TaskSatelliteBackupExtension" and @item="TryGetSatelliteIDListAsync"]'/>
        protected override Task<List<Guid>> TryGetSatelliteIDListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.GetSecondarySatellitesIDsAsync(mainCardID, dbScope, cancellationToken);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.AddSatelliteToList(mainCard, satellite, KrConstants.KrSecondarySatelliteListInfoKey);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.ClearSatelliteList(mainCard, KrConstants.KrSecondarySatelliteListInfoKey);
    }
}