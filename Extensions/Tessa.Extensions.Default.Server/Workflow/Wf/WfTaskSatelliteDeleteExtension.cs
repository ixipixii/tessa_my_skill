using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Расширение для удаления сателлитов задач при удалении карточки.
    /// </summary>
    public class WfTaskSatelliteDeleteExtension :
        TaskSatelliteDeleteExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="krTypesCache">Кэш с информацией по типам документов.</param>
        /// <param name="extendedRepositoryWithoutTransaction">
        /// Репозиторий для управления карточками с расширениями, но без транзакции.
        /// </param>
        /// <param name="contentStrategy">Стратегия для управления контентом файлов.</param>
        /// <param name="storeStrategy">Стратегия по сохранению карточки.</param>
        public WfTaskSatelliteDeleteExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardContentStrategy contentStrategy,
            ICardStoreStrategy storeStrategy)
            : base(extendedRepositoryWithoutTransaction, contentStrategy, storeStrategy)
        {
            Check.ArgumentNotNull(krTypesCache, nameof(krTypesCache));

            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.WfTaskCardTypeID;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteFileInfoListKey"]'/>
        protected override string TaskSatelliteFileInfoListKey => WfHelper.TaskSatelliteFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TaskSatelliteMovedFileInfoListKey"]'/>
        protected override string TaskSatelliteMovedFileInfoListKey => WfHelper.TaskSatelliteMovedFileInfoListKey;

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetTaskSatelliteList(mainCard);

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="CreateSatelliteInfoAsync"]'/>
        protected override async ValueTask<SatelliteInfo> CreateSatelliteInfoAsync(Card satelliteCard, CancellationToken cancellationToken = default) =>
            WfHelper.CreateSatelliteInfo(satelliteCard);

        /// <doc path='info[@type="TaskSatelliteDeleteExtension" and @item="TryGetSatelliteInfoListAsync"]'/>
        protected override Task<List<SatelliteInfo>> TryGetSatelliteInfoListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatelliteInfoListAsync(dbScope, mainCardID, this.SatelliteTypeID, cancellationToken);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="GetAdditionalInfoForDeletionAsync"]'/>
        protected override async ValueTask<object> GetAdditionalInfoForDeletionAsync(ICardDeleteExtensionContext context) =>
            KrToken.TryGet(context.Request.Info);

        /// <doc path='info[@type="MultitypeSatelliteDeleteExtension" and @item="PrepareSatelliteDeleteRequest"]'/>
        protected override async ValueTask PrepareSatelliteDeleteRequestAsync(
            ICardDeleteExtensionContext context,
            CardDeleteRequest request,
            object additionalInfo) =>
            ((KrToken) additionalInfo)?.Set(request.Info);

        #endregion
    }
}