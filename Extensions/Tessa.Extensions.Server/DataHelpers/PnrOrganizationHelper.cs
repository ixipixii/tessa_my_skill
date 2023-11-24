using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrOrganizationHelper
    {
        /// <summary>
        /// Организация по ID
        /// </summary>
        public static async Task<PnrOrganization> GetOrganizationByID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 *
                            FROM PnrOrganizations with(nolock)
                            WHERE ID = @id",
                        db.Parameter("@id", id))
                    .ExecuteAsync<PnrOrganization>();
            }
        }

        /// <summary>
        /// Организация по MDM-ключу
        /// </summary>
        public static async Task<PnrOrganization> GetOrganizationByMDM(IDbScope dbScope, string mdmKey)
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
                            FROM PnrOrganizations with(nolock)
                            WHERE MDMKey = @MDMKey",
                        db.Parameter("@MDMKey", mdmKey))
                    .ExecuteAsync<PnrOrganization>();
            }
        }
    }
}
