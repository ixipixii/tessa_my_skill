﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrDialogSatelliteBackupExtension : MultitypeSatelliteBackupExtension
    {
        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Constructor

        /// <inheritdoc />
        public KrDialogSatelliteBackupExtension(
            ICardRepository cardRepository,
            IKrTypesCache krTypesCache)
            : base(cardRepository)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.krTypesCache);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.GetDialogSatelliteInfosAsync(mainCardID, dbScope, cancellationToken);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.AddSatelliteToList(mainCard, satellite, KrConstants.KrDialogSatelliteListInfoKey);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.ClearSatelliteList(mainCard, KrConstants.KrDialogSatelliteListInfoKey);

        #endregion
    }
}