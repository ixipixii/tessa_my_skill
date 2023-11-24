using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    /// <summary>
    /// Заявка на контрагента
    /// </summary>
    public static class PnrPartnerRequestHelper
    {
        /// <summary>
        /// Заявка по ID
        /// </summary>
        public static async Task<PnrPartnerRequest> GetPartnerRequestByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrPartnerRequests with(nolock)
                            WHERE ID = @id",
                        db.Parameter("@id", id))
                    .ExecuteAsync<PnrPartnerRequest>();
            }
        }

        /// <summary>
        /// Заявки, связанные с указанным КА
        /// </summary>
        public static async Task<List<PnrPartnerRequest>> GetPartnerRequestsByPartnerID(IDbScope dbScope, Guid partnerID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"select
                                dci.ID,
                                dci.FullNumber,
                                dci.CreationDate,
                                fsci.StateName,
                                r.ShortName,
                                r.FullName,
                                r.RequestTypeID,
                                r.RequestTypeName,
                                r.PartnerID,
                                r.PartnerName,
                                r.INN,
                                r.KPP
                            from PnrPartnerRequests r with(nolock)
                            join DocumentCommonInfo dci with(nolock) on dci.ID = r.ID
                            left join FdSatelliteCommonInfo fsci with(nolock) on fsci.MainCardId = r.ID
                            where r.PartnerID = @partnerID
                            order by r.RequestTypeID, dci.CreationDate DESC",
                            db.Parameter("@partnerID", partnerID))
                    .ExecuteListAsync<PnrPartnerRequest>();
            }
        }
    }

}
