using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrPartnerHelper
    {
        /// <summary>
        /// Контрагент по ID
        /// </summary>
        public static async Task<PnrPartner> GetPartnerByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM Partners with(nolock)
                            WHERE ID = @id",
                        db.Parameter("@id", id))
                    .ExecuteAsync<PnrPartner>();
            }
        }

        /// <summary>
        /// Контрагент по MDM-ключу
        /// </summary>
        public static async Task<PnrPartner> GetPartnerByMDM(IDbScope dbScope, string mdmKey)
        {
            if (string.IsNullOrEmpty(mdmKey))
            {
                return null;
            }

            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM Partners with(nolock)
                            WHERE MDMKey = @MDMKey",
                        db.Parameter("@MDMKey", mdmKey))
                    .ExecuteAsync<PnrPartner>();
            }
        }
    }
}
