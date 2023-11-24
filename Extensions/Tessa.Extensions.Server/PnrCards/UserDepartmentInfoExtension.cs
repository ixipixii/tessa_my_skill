using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class UserDepartmentInfoExtension : CardRequestExtension
    {
        private readonly IDbScope dbScope;
        public UserDepartmentInfoExtension(IDbScope dbScope)
        {
            this.dbScope = dbScope;
        }

        public override Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
            }

            Guid authorID = context.Request.Info.Get<Guid>("AuthorID");
            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;

                db.SetCommand(@"
select
	t2.ID,
	t2.CFOID,
	t2.CFOName,
    t2.Name,
	t2.Idx
from (	
	SELECT TOP 1 RU.ID, DR.CFOID, DR.CFOName, R.Name, R.Idx FROM RoleUsers RU with(nolock) 
	left join DepartmentRoles DR with(nolock) on DR.ID = RU.ID
	left join Roles R with(nolock) on R.ID = RU.ID
	where RU.UserID = @ID AND RU.UserID <> RU.ID
) t2",
                db.Parameter("@ID", authorID))
                .LogCommand();

                // получить первое значение (подразделений может быть и несколько)
                using (var reader = db.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Guid departmentID = reader.GetValue<Guid>(0);
                        Guid? cfoID = reader.GetValue<Guid?>(1);
                        string cfoName = reader.GetValue<string>(2);
                        string name = reader.GetValue<string>(3);
                        string index = reader.GetValue<string>(4);

                        context.Response.Info["DepartmentID"] = departmentID;
                        context.Response.Info["CFOID"] = cfoID;
                        context.Response.Info["CFOName"] = cfoName;
                        context.Response.Info["Name"] = name;
                        context.Response.Info["Index"] = index;
                    }
                    else
                    {
                        context.Response.Info["DepartmentID"] = null;
                        context.Response.Info["CFOID"] = null;
                        context.Response.Info["CFOName"] = null;
                        context.Response.Info["Name"] = null;
                        context.Response.Info["Index"] = null;
                    }
                }
            }

            return base.AfterRequest(context);
        }
    }
}
