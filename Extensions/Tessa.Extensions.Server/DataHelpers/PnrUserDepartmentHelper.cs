using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrUserDepartmentHelper
    {
        public static async Task<PnrDepartment> GetDepartmentByUserID(IDbScope dbScope, Guid authorID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                        @"SELECT TOP 1 
                            DR.ID, 
                            DR.CFOID, 
                            DR.CFOName, 
                            R.Name, 
                            R.Idx 
                            FROM DepartmentRoles as DR WITH(NOLOCK) 
                            LEFT JOIN RoleUsers as RU WITH(NOLOCK) ON DR.ID = RU.ID 
                            LEFT JOIN Roles as R WITH(NOLOCK) ON R.ID = DR.ID 
                            WHERE RU.UserID = @authorID AND RU.UserID <> RU.ID;",
                        db.Parameter("authorID", authorID))
                    .ExecuteAsync<PnrDepartment>();
            }
        }
    }
}
