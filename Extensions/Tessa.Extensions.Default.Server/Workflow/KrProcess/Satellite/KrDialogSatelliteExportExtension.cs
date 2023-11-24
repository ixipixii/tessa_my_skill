using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrDialogSatelliteExportExtension : 
        MultitypeSatelliteExportExtension
    {
        #region Fields

        private readonly IKrTypesCache typesCache;

        #endregion

        #region Constructor

        /// <inheritdoc />
        public KrDialogSatelliteExportExtension(
            ICardRepository cardRepository,
            IKrTypesCache typesCache)
            : base(cardRepository)
        {
            this.typesCache = typesCache;
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.typesCache);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.GetDialogSatelliteInfosAsync(mainCardID, dbScope, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.AddSatelliteToList(mainCard, satellite, KrConstants.KrDialogSatelliteListInfoKey);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.ClearSatelliteList(mainCard, KrConstants.KrDialogSatelliteListInfoKey);

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