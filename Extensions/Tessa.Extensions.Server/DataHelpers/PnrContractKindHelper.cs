using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrContractKindHelper
    {
        /// <summary>
        /// Вид договора по ID
        /// </summary>
        public static async Task<PnrContractKind> GetContractKindByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrContractKinds with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", id))
                    .ExecuteAsync<PnrContractKind>();
            }
        }
    }
}
