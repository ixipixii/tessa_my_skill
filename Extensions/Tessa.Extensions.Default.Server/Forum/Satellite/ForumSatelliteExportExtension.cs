using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Forums;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Forum.Satellite
{
    /// <summary>
    /// Экспорт карточки вместе с карточкой-сателлитом форумов, если она была создана.
    /// </summary>
    public class ForumSatelliteExportExtension : 
        MultitypeSatelliteExportExtension
    {
        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Constructors

        public ForumSatelliteExportExtension(
            ICardRepository cardRepository,
            IKrTypesCache krTypesCache)
            : base(cardRepository)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            FmHelper.GetForumSatelliteInfosAsync(mainCardID, dbScope, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.AddSatelliteToList(mainCard, satellite, FmHelper.FmSatelliteInfoKey);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.ClearSatelliteList(mainCard, FmHelper.FmSatelliteInfoKey);

        public override Task AfterRequest(ICardGetExtensionContext context)
        {
            // Защита от экспорта при копировании
            if (context.Request.TryGetInfo()?.ContainsKey(CardHelper.CopyingCardKey) == true)
            {
                return Task.CompletedTask;
            }

            return base.AfterRequest(context);
        }

        #endregion
    }
}