using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class GetOrganizationHead : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }
            Guid organizationID = context.Request.Info.TryGet<Guid>("organizationID");

            var organization = await PnrOrganizationHelper.GetOrganizationByID(context.DbScope, organizationID);
            context.Response.Info.Add("HeadLegalEntityID", organization.HeadLegalEntityID);
            context.Response.Info.Add("HeadLegalEntityName", organization.HeadLegalEntityName);
        }
    }
}
