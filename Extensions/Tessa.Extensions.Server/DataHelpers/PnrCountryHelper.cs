using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrCountryHelper
    {
        /// <summary>
        /// Страна по MDM-ключу
        /// </summary>
        public static async Task<PnrCountry> GetCountryByMDM(IDbScope dbScope, string mdmKey)
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
                            FROM PnrCountries with(nolock)
                            WHERE MDMKey = @MDMKey",
                        db.Parameter("@MDMKey", mdmKey))
                    .ExecuteAsync<PnrCountry>();
            }
        }

        /// <summary>
        /// Страна по ID
        /// </summary>
        public static async Task<PnrCountry> GetCountryByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrCountries with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", id))
                    .ExecuteAsync<PnrCountry>();
            }
        }
    }
}
