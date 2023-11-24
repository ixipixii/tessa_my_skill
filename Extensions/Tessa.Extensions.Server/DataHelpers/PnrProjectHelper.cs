using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrProjectHelper
    {
        /// <summary>
        /// Проект по ID
        /// </summary>
        public static async Task<PnrProject> GetProjectByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrProjects with(nolock)
                            WHERE ID = @id",
                        db.Parameter("@id", id))
                    .ExecuteAsync<PnrProject>();
            }
        }

        /// <summary>
        /// Проект по MDM-ключу
        /// </summary>
        public static async Task<PnrProject> GetProjectByMDM(IDbScope dbScope, string mdmKey)
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
                            FROM PnrProjects with(nolock)
                            WHERE MDMKey = @MDMKey",
                        db.Parameter("@MDMKey", mdmKey))
                    .ExecuteAsync<PnrProject>();
            }
        }
    }
}
