using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Расширение по сохранению карточки-сателлита для задания.
    /// </summary>
    public class WfTaskSatelliteStoreExtension :
        TaskSatelliteStoreExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="extendedRepository">Репозиторий для управления карточками с расширениями и с транзакцией.</param>
        /// <param name="extendedRepositoryWithoutTransaction">Репозиторий для управления карточками с расширениями, но без транзакции.</param>
        /// <param name="cardTransactionStrategy">Стратегия обеспечения блокировок для взаимодействия с основной карточкой.</param>
        /// <param name="cardGetStrategy">Стратегия низкоуровневой загрузки карточки, используемая при загрузке виртуального задания.</param>
        public WfTaskSatelliteStoreExtension(
            ICardRepository extendedRepository,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardTransactionStrategy cardTransactionStrategy,
            ICardGetStrategy cardGetStrategy)
            : base(
                extendedRepository,
                extendedRepositoryWithoutTransaction,
                cardTransactionStrategy,
                cardGetStrategy)
        {
        }

        #endregion

        #region Protected Declarations

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.WfTaskCardTypeID;

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="NextCardIDKey"]'/>
        protected override string NextCardIDKey => WfHelper.NextCardIDKey;

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="NextCardTypeIDKey"]'/>
        protected override string NextCardTypeIDKey => WfHelper.NextCardTypeIDKey;

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="VirtualMainCardIDKey"]'/>
        protected override string VirtualMainCardIDKey => WfHelper.VirtualMainCardIDKey;

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="TryGetTaskSatelliteIDAsync"]'/>
        protected override Task<Guid?> TryGetTaskSatelliteIDAsync(IDbScope dbScope, Guid taskRowID, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetTaskSatelliteIDAsync(dbScope, taskRowID, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="TryGetMainCardIDAndTaskRowIDAsync"]'/>
        protected override Task<(bool result, Guid mainCardID, Guid taskRowID)> TryGetMainCardIDAndTaskRowIDAsync(
            IDbScope dbScope,
            Guid satelliteID,
            CancellationToken cancellationToken = default) =>
            WfHelper.TryGetMainCardIDAndTaskRowIDAsync(dbScope, satelliteID, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="CanModifyTaskCardAsync"]'/>
        protected override async ValueTask<bool> CanModifyTaskCardAsync(
            ICardStoreExtensionContext context,
            Card mainCard,
            Guid taskRowID) =>
            WfHelper.CanModifyTaskCard(mainCard);

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="SetupVirtualSatelliteAsync"]'/>
        protected override async ValueTask SetupVirtualSatelliteAsync(
            ICardStoreExtensionContext context,
            Card satellite,
            Guid mainCardID,
            Guid taskRowID)
        {
            Dictionary<string, object> fields = satellite.Sections[WfHelper.TaskSatelliteSection].RawFields;
            fields[WfHelper.TaskSatelliteTaskRowIDField] = taskRowID;
            fields[WfHelper.TaskSatelliteMainCardIDField] = mainCardID;
        }

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="IsMainCardFileAsync"]'/>
        protected override async ValueTask<bool> IsMainCardFileAsync(
            ICardStoreExtensionContext context,
            Card satellite,
            CardFile file) =>
            file.CategoryID == WfHelper.MainCardCategoryID;

        /// <doc path='info[@type="TaskSatelliteStoreExtension" and @item="PrepareMainCardFileToStoreAsync"]'/>
        protected override async ValueTask PrepareMainCardFileToStoreAsync(
            ICardStoreExtensionContext context,
            Card satellite,
            CardFile file)
        {
            // файлы основной карточки будут добавлены с пустой категорией
            file.CategoryID = null;
            file.CategoryCaption = null;
        }

        #endregion
    }
}