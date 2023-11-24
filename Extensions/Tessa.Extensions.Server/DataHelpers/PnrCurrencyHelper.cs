using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrCurrencyHelper
    {
        /// <summary>
        /// Валюта по MDM-ключу
        /// </summary>
        public static async Task<PnrCurrency> GetCurrencyByMDM(IDbScope dbScope, string mdmKey)
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
                            FROM Currencies with(nolock)
                            WHERE MDMKey = @MDMKey",
                        db.Parameter("@MDMKey", mdmKey))
                    .ExecuteAsync<PnrCurrency>();
            }
        }

        /// <summary>
        /// Валюта по ID
        /// </summary>
        public static async Task<PnrCurrency> GetCurrencyByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM Currencies with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", id))
                    .ExecuteAsync<PnrCurrency>();
            }
        }
    }
}
