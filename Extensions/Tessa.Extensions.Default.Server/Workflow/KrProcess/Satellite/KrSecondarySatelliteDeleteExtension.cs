using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrSecondarySatelliteDeleteExtension : TaskSatelliteDeleteExtension
    {
        private readonly IKrTypesCache typesCache;

        /// <inheritdoc />
        public KrSecondarySatelliteDeleteExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardContentStrategy contentStrategy,
            ICardStoreStrategy storeStrategy,
            IKrTypesCache typesCache)
            : base(extendedRepositoryWithoutTransaction, contentStrategy, storeStrategy)
        {
            this.typesCache = typesCache;
        }

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.KrSecondarySatelliteTypeID;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteFileInfoListKey"]'/>
        protected override string TaskSatelliteFileInfoListKey => KrConstants.TaskSatelliteFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteMovedFileInfoListKey"]'/>
        protected override string TaskSatelliteMovedFileInfoListKey => KrConstants.TaskSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.typesCache);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCardList(mainCard, KrConstants.KrSecondarySatelliteListInfoKey);

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            KrProcessHelper.CreateSatelliteInfo(satelliteCard);

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.TryGetSecondarySatelliteInfoListAsync(mainCardID, dbScope, this.SatelliteTypeID, cancellationToken);
    }
}