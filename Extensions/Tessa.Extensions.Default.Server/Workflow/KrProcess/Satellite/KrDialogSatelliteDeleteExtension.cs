using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrDialogSatelliteDeleteExtension : MultitypeSatelliteDeleteExtension
    {
        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        /// <inheritdoc />
        public KrDialogSatelliteDeleteExtension(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardContentStrategy contentStrategy,
            ICardStoreStrategy storeStrategy,
            IKrTypesCache krTypesCache)
            : base(extendedRepositoryWithoutTransaction, contentStrategy, storeStrategy)
        {
            this.krTypesCache = krTypesCache;
        }

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteFileInfoListKey"]'/>
        protected override string TaskSatelliteFileInfoListKey => KrConstants.DialogSatelliteFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteMovedFileInfoListKey"]'/>
        protected override string TaskSatelliteMovedFileInfoListKey => KrConstants.DialogSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsSatelliteCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsSatelliteCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default)
        {
            var schemeItems = cardType.SchemeItems;
            foreach (var schemeItem in schemeItems)
            {
                if (schemeItem.SectionID != KrConstants.KrDialogSatellite.SectionID)
                {
                    continue;
                }

                const int requiredColumnsCount = 2;
                var cnt = 0;
                foreach (var columnID in schemeItem.ColumnIDList)
                {
                    if (columnID == KrConstants.KrDialogSatellite.MainCardIDFieldID
                        || columnID == KrConstants.KrDialogSatellite.TypeIDFieldID)
                    {
                        cnt++;
                    }

                    if (cnt == requiredColumnsCount)
                    {
                        return true;
                    }
                }

                return cnt == requiredColumnsCount;
            }

            return false;
        }

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.krTypesCache);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCardList(mainCard, KrConstants.KrDialogSatelliteListInfoKey);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            KrProcessHelper.CreateSatelliteInfo(satelliteCard);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.GetDialogSatelliteInfosAsync(mainCardID, dbScope,  cancellationToken);
    }
}