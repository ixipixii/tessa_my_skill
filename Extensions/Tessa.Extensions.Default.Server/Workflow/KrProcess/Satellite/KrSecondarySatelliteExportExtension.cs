using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrSecondarySatelliteExportExtension : TaskSatelliteExportExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="krTypesCache">Кэш типов карточек и документов в типовом решении.</param>
        /// <param name="cardRepository">Репозиторий для управления карточками с расширениями и транзакцией.</param>
        public KrSecondarySatelliteExportExtension(
            IKrTypesCache krTypesCache,
            ICardRepository cardRepository)
            : base(cardRepository)
        {
            Check.ArgumentNotNull(krTypesCache, nameof(krTypesCache));

            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.KrSecondarySatelliteTypeID;

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.krTypesCache);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="TryGetSatelliteIDListAsync"]'/>
        protected override Task<List<Guid>> TryGetSatelliteIDListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            KrProcessHelper.GetSecondarySatellitesIDsAsync(mainCardID, dbScope, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.AddSatelliteToList(mainCard, satellite, KrConstants.KrSecondarySatelliteListInfoKey);

        /// <doc path='info[@type="TaskSatelliteExportExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.ClearSatelliteList(mainCard, KrConstants.KrSecondarySatelliteListInfoKey);

        #endregion
    }
}