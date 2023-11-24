using System;
using System.Threading.Tasks;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrIsUserInRole
    {
        /// <summary>
        /// Входит ли укзанный пользователь в указанную роль
        /// </summary>
        public static async Task<bool> GetIsUserInRole(IDbScope dbScope, Guid? userID, Guid? roleID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return Convert.ToBoolean(
                    await db.SetCommand(@"
                        SELECT
                            COUNT(*)
                        FROM
                            RoleUsers
                        WHERE 1=1
                            AND UserID = @userID
                            AND ID = @roleID",
                            db.Parameter("userID", userID),
                            db.Parameter("roleID", roleID))
                    .LogCommand()
                    .ExecuteAsync<int>());
            }
        }
    }
}
