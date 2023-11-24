using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrSecondarySatelliteRestoreExtension : TaskSatelliteRestoreExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="krTypesCache">Кэш типов карточек и документов в типовом решении.</param>
        /// <param name="extendedRepositoryWithoutTransaction">
        /// Репозиторий для управления карточками с расширениями, но без транзакции.
        /// </param>
        /// <param name="cardMetadata">Метаданные по типам карточек.</param>
        /// <param name="contentStrategy">Стратегия для управления контентом файлов.</param>
        /// <param name="getStrategy">Стратегия по загрузке карточки.</param>
        /// <param name="storeStrategy">Стратегия по сохранению карточки.</param>
        public KrSecondarySatelliteRestoreExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository extendedRepositoryWithoutTransaction,
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
        protected override string TaskSatelliteMovedFileInfoListKey => KrConstants.TaskSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override async ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            KrComponentsHelper.HasBase(cardType.ID, this.krTypesCache);

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteCardList(mainCard, KrConstants.KrSecondarySatelliteListInfoKey);

        /// <doc path='info[@type="TaskSatelliteRestoreExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            KrProcessHelper.CreateSatelliteInfo(satelliteCard);

        #endregion
    }
}