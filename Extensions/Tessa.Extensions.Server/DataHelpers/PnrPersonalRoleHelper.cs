using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrPersonalRoleHelper
    {
        /// <summary>
        /// Сотрудник по Email
        /// </summary>
        public static async Task<PnrPersonalRole> GetPersonalRoleByEmail(IDbScope dbScope, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 ID, Name, Email
                            FROM PersonalRoles with(nolock)
                            WHERE Email = @MDMKey",
                        db.Parameter("@MDMKey", email))
                    .ExecuteAsync<PnrPersonalRole>();
            }
        }
    }
}
