using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class LegalEntityIndexExtension : CardRequestExtension
    {
        private readonly IDbScope dbScope;
        public LegalEntityIndexExtension(IDbScope dbScope)
        {
            this.dbScope = dbScope;
        }

        public override Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
            }

            Guid organizationID = context.Request.Info.Get<Guid>("OrganizationID");
            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;

                db.SetCommand(@"
select CodeID, CodeName, CodeIdx
from PnrOrganizations
where ID = @ID",
                db.Parameter("@ID", organizationID))
                .LogCommand();

                // получить первое значение (подразделений может быть и несколько)
                using (var reader = db.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Guid codeID = reader.GetValue<Guid>(0);
                        string codeName = reader.GetValue<string>(1);
                        string codeIdx = reader.GetValue<string>(2);

                        context.Response.Info["CodeID"] = codeID;
                        context.Response.Info["CodeName"] = codeName;
                        context.Response.Info["CodeIdx"] = codeIdx;
                    }
                    else
                    {
                        context.Response.Info["CodeID"] = null;
                        context.Response.Info["CodeName"] = null;
                        context.Response.Info["CodeIdx"] = null;
                    }
                }
            }

            return base.AfterRequest(context);
        }
    }
}
