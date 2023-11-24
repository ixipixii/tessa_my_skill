using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class GetUserDepartmentInfoRequest : CardRequestExtension
    {
        // данные по подразделению пользователя
        private class UserDepartmentInfo
        {
            public Guid? ID { get; set; }
            public Guid? CFOID { get; set; }
            public string CFOName { get; set; }
            public string Name { get; set; }
            public string Idx { get; set; }
        }

        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            context.Response.Info.Add("DepartmentID", null);
            context.Response.Info.Add("CFOID", null);
            context.Response.Info.Add("CFOName", null);
            context.Response.Info.Add("Name", null);
            context.Response.Info.Add("Index", null);

            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Guid? authorID = context.Request.Info.TryGet<Guid?>("authorID");
            if (authorID == null)
            {
                return;
            }

            var userDepartmentInfo = new UserDepartmentInfo();
            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                userDepartmentInfo = await db.SetCommand(
                    "SELECT TOP 1 "
                        + "DR.ID, "
                        + "DR.CFOID, "
                        + "DR.CFOName, "
                        + "R.Name, "
                        + "R.Idx "
                    + "FROM DepartmentRoles as DR WITH(NOLOCK) "
                    + "LEFT JOIN RoleUsers as RU WITH(NOLOCK) ON DR.ID = RU.ID "
                    + "LEFT JOIN Roles as R WITH(NOLOCK) ON R.ID = DR.ID "
                    + "WHERE RU.UserID = @authorID AND RU.UserID <> RU.ID;",
                db.Parameter("authorID", authorID))
                .LogCommand()
                .ExecuteAsync<UserDepartmentInfo>();
            }
            
            if(userDepartmentInfo != null)
            {
                context.Response.Info["DepartmentID"] = userDepartmentInfo.ID;
                context.Response.Info["CFOID"] = userDepartmentInfo.CFOID;
                context.Response.Info["CFOName"] = userDepartmentInfo.CFOName;
                context.Response.Info["Name"] = userDepartmentInfo.Name;
                context.Response.Info["Index"] = userDepartmentInfo.Idx;
            }
        }
    }
}
