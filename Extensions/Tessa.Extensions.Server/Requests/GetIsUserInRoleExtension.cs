using System;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class GetIsUserInRoleExtension : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Guid? userID = context.Request.Info.TryGet<Guid?>("userID");
            Guid? roleID = context.Request.Info.TryGet<Guid?>("roleID");

            if (userID == null || roleID == null)
            {
                return;
            }

            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                var isUserInRole = Convert.ToBoolean(
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
                
                context.Response.Info["isUserInRole"] = isUserInRole;
            }
        }
    }
}
