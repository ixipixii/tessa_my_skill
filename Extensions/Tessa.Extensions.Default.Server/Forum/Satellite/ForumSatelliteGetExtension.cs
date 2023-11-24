using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions.Templates;
using Tessa.Forums;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Forum.Satellite
{
    public class ForumSatelliteGetExtension :
        CardSatelliteGetExtension
    {
        #region Constructors

        public ForumSatelliteGetExtension(
            ICardRepository cardRepository,
            ICardTransactionStrategy cardTransactionStrategy)
            : base(cardRepository, cardTransactionStrategy)
        {
        }

        #endregion

        #region Base Overrides

        protected override Guid SatelliteTypeID => ForumHelper.ForumSatelliteTypeID;

        protected override Task<Guid?> TryGetSatelliteIDAsync(IDbScope dbScope, Guid mainCardID, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetSatelliteIDAsync(
                dbScope,
                mainCardID,
                FmSections.ForumSatelliteName,
                ForumHelper.MainCardID,
                cancellationToken);

        protected override async ValueTask SetSatelliteMainCardIDAsync(Card satellite, Guid mainCardID, CancellationToken cancellationToken = default)
        {
            satellite.Sections[FmSections.ForumSatelliteName].RawFields[ForumHelper.MainCardID] = mainCardID;
        }

        #endregion
    }
}
