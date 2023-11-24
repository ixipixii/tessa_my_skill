using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Загрузка карточки-сателлита Workflow по идентификатору основной карточки.
    /// </summary>
    public sealed class WfSatelliteGetExtension :
        CardSatelliteGetExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="cardRepository">Репозиторий для управления карточками с расширениями и транзакцией.</param>
        /// <param name="cardTransactionStrategy">Стратегия обеспечения блокировок для взаимодействия с основной карточкой.</param>
        public WfSatelliteGetExtension(
            ICardRepository cardRepository,
            ICardTransactionStrategy cardTransactionStrategy)
            : base(cardRepository, cardTransactionStrategy)
        {
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="CardSatelliteGetExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.WfSatelliteTypeID;

        /// <doc path='info[@type="CardSatelliteGetExtension" and @item="TryGetSatelliteIDAsync"]'/>
        protected override Task<Guid?> TryGetSatelliteIDAsync(IDbScope dbScope, Guid mainCardID, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetSatelliteIDAsync(dbScope, mainCardID, cancellationToken);

        /// <doc path='info[@type="CardSatelliteGetExtension" and @item="SetSatelliteMainCardIDAsync"]'/>
        protected override async ValueTask SetSatelliteMainCardIDAsync(Card satellite, Guid mainCardID, CancellationToken cancellationToken = default) =>
            WfHelper.SetSatelliteMainCardID(satellite, mainCardID);

        #endregion
    }
}
