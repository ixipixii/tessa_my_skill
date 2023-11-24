using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Расширение по импорту карточки с сателлитами задач.
    /// </summary>
    public class WfTaskSatelliteImportExtension :
        TaskSatelliteImportExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="krTypesCache">Кэш типов карточек и документов в типовом решении.</param>
        /// <param name="extendedRepositoryWithoutTransaction">
        /// Репозиторий для управления карточками с расширениями и без транзакции.
        /// </param>
        public WfTaskSatelliteImportExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository extendedRepositoryWithoutTransaction)
            : base(extendedRepositoryWithoutTransaction)
        {
            Check.ArgumentNotNull(krTypesCache, nameof(krTypesCache));

            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteImportExtension" and @item="IsMainCardTypeAsync"]'/>
        protected override ValueTask<bool> IsMainCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default) =>
            WfHelper.TypeSupportsWorkflowAsync(this.krTypesCache, cardType, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteImportExtension" and @item="TryGetSatelliteCardListAsync"]'/>
        protected override async ValueTask<List<Card>> TryGetSatelliteCardListAsync(Card mainCard, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetTaskSatelliteList(mainCard);

        #endregion
    }
}
