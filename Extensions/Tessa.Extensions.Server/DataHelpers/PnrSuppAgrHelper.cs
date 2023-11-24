using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrSuppAgrHelper
    {
        /// <summary>
        /// Карточка ДС по ID
        /// </summary>
        public static async Task<PnrSuppAgr> GetSuppAgrByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrSupplementaryAgreements with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", id))
                    .ExecuteAsync<PnrSuppAgr>();
            }
        }
    }
}
