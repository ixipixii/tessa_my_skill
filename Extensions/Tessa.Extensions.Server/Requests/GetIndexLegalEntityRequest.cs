using System;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class GetIndexLegalEntityRequest : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }
            Guid? organizationID = context.Request.Info.TryGet<Guid?>("organizationID");
            if (organizationID == null)
            {
                context.Response.Info.Add("LegalEntityID", null);
                return;
            }
            var legalEntity = new LegalEntityInfo();
            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                legalEntity = await db.SetCommand(
                    context.DbScope.BuilderFactory
                    //.Select().Top(1).C("t", "CodeID", "CodeIdx", "CodeName")
                    .Select().Top(1).C("t", "LegalEntityIndex")
                    .From("PnrOrganizations", "t").NoLock()
                    .Where().C("ID").Equals().P("organizationID")
                    .Build(),
                db.Parameter("organizationID", organizationID))
                .LogCommand()
                .ExecuteAsync<LegalEntityInfo>();
            }
            //context.Response.Info.Add("LegalEntityID", legalEntity.CodeID);
            //context.Response.Info.Add("LegalEntityName", legalEntity.CodeName);
            context.Response.Info.Add("LegalEntityIdx", legalEntity.LegalEntityIndex);
        }

        private class LegalEntityInfo
        {
            public Guid? CodeID { get; set; }

            public string CodeIdx { get; set; }

            public string CodeName { get; set; }
            public string LegalEntityIndex { get; set; }
        }
    }
}
