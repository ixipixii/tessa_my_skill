using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Forums;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Unity;

namespace Tessa.Extensions.Default.Server.Forum.Satellite
{
    /// <summary>
    /// Восстановление карточки-сателлита форумов при восстановлении основной карточки.
    /// </summary>
    public class ForumSatelliteRestoreExtension :
        TaskSatelliteRestoreExtension
    {
        #region Constructors

        public ForumSatelliteRestoreExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository extendedRepositoryWithoutTransaction,
            ICardMetadata cardMetadata,
            ICardContentStrategy contentStrategy,
            ICardGetStrategy getStrategy,
            ICardStoreStrategy storeStrategy)
            : base(extendedRepositoryWithoutTransaction, cardMetadata, contentStrategy, getStrategy, storeStrategy)
        {
            Check.ArgumentNotNull(krTypesCache, nameof(krTypesCache));

            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="TaskSatelliteMovedFileInfoListKey"]'/>
        protected override string TaskSatelliteMovedFileInfoListKey => FmHelper.FmSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.GetKrComponents(cardType.ID, this.krTypesCache).Has(KrComponents.Base);

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCardList(mainCard, FmHelper.FmSatelliteInfoKey);

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            KrProcessHelper.CreateSatelliteInfo(satelliteCard);

        #endregion
    }
}