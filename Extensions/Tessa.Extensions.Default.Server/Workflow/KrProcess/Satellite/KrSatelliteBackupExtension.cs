using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    /// <summary>
    /// Загрузка карточки-сателлита при создании резервной копии основной карточки.
    /// </summary>
    public sealed class KrSatelliteBackupExtension : CardSatelliteBackupExtension
    {
        #region fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Constructors

        public KrSatelliteBackupExtension(ICardRepository cardRepository,
            IKrTypesCache krTypesCache)
            : base(cardRepository)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Base Overrides

        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base);

        protected override Guid SatelliteTypeID => DefaultCardTypes.KrSatelliteTypeID;

        protected override Task<Guid?> TryGetSatelliteIDAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteIDAsync(
                dbScope,
                mainCardID,
                KrConstants.KrApprovalCommonInfo.Name,
                KrConstants.KrProcessCommonInfo.MainCardID,
                cancellationToken);

        protected override async ValueTask SetSatelliteAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.SetSatellite(mainCard, satellite, KrConstants.KrSatelliteInfoKey);

        #endregion
    }
}
