using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Forums;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Forum.Satellite
{
    public class ForumSatelliteDeleteExtension :
        MultitypeSatelliteDeleteExtension
    {
        #region Constructors

        public ForumSatelliteDeleteExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardContentStrategy contentStrategy,
            ICardStoreStrategy storeStrategy,
            IKrTypesCache krTypesCache)
            : base(extendedRepositoryWithoutTransaction, contentStrategy, storeStrategy)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides
        
        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteFileInfoListKey"]'/>
        protected override string TaskSatelliteFileInfoListKey => FmHelper.FmSatelliteFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteMovedFileInfoListKey"]'/>
        protected override string TaskSatelliteMovedFileInfoListKey => FmHelper.FmSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsSatelliteCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsSatelliteCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default)
        {
            return cardType.ID == ForumHelper.ForumSatelliteTypeID;
        }

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCardList(mainCard, FmHelper.FmSatelliteInfoKey);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            KrProcessHelper.CreateSatelliteInfo(satelliteCard);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            FmHelper.GetForumSatelliteInfosAsync(mainCardID, dbScope, cancellationToken);

        #endregion
    }
}