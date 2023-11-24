using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Расширение для загрузки сателлитов задач при удалении карточки в корзину.
    /// </summary>
    public class WfTaskSatelliteBackupExtension :
        TaskSatelliteBackupExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="krTypesCache">Кэш типов карточек и документов в типовом решении.</param>
        /// <param name="cardRepository">Репозиторий для управления карточками с расширениями и транзакцией.</param>
        public WfTaskSatelliteBackupExtension(
            IKrTypesCache krTypesCache,
            ICardRepository cardRepository)
            : base(cardRepository)
        {
            Check.ArgumentNotNull(krTypesCache, nameof(krTypesCache));

            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Кэш типов карточек и документов в типовом решении.
        /// </summary>
        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteBackupExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.WfTaskCardTypeID;

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteBackupExtension" and @item="TryGetSatelliteIDListAsync"]'/>
        protected override Task<List<Guid>> TryGetSatelliteIDListAsync(
            IDbScope dbScope,
            Guid mainCardID,
            CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatelliteIDListAsync(dbScope, mainCardID, cancellationToken);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="AddSatelliteToListAsync"]'/>
        protected override async ValueTask AddSatelliteToListAsync(Card mainCard, Card satellite, CancellationToken cancellationToken = default) =>
            WfHelper.AddTaskSatelliteToList(mainCard, satellite);

        /// <doc path='info[@type="MultitypeSatelliteBackupExtension" and @item="ClearSatelliteListAsync"]'/>
        protected override async ValueTask ClearSatelliteListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.ClearTaskSatelliteList(mainCard);

        #endregion
    }
}