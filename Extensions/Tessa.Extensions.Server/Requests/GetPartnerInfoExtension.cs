using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class GetPartnerInfoExtension : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Guid partnerID = context.Request.Info.Get<Guid>("partnerID");
            var partner = await PnrPartnerHelper.GetPartnerByID(context.DbScope, partnerID);
            if (partner == null)
            {
                return;
            }
            
            context.Response.Info["TypeID"] = partner.TypeID;
            context.Response.Info["TypeName"] = partner.TypeName;
            context.Response.Info["FullName"] = partner.FullName;
            context.Response.Info["Name"] = partner.Name;
            context.Response.Info["SpecialSignID"] = partner.SpecialSignID;
            context.Response.Info["SpecialSignName"] = partner.SpecialSignName;
            context.Response.Info["NonResident"] = partner.NonResident;

            context.Response.Info["INN"] = partner.INN;
            context.Response.Info["KPP"] = partner.KPP;

            context.Response.Info["CountryRegistrationID"] = partner.CountryRegistrationID;
            context.Response.Info["CountryRegistrationName"] = partner.CountryRegistrationName;
            context.Response.Info["Comment"] = partner.Comment;
            context.Response.Info["DirectionID"] = partner.DirectionID;
            context.Response.Info["DirectionName"] = partner.DirectionName;

            context.Response.Info["IdentityDocument"] = partner.IdentityDocument;
            context.Response.Info["IdentityDocumentKind"] = partner.IdentityDocumentKind;
            context.Response.Info["IdentityDocumentIssueDate"] = partner.IdentityDocumentIssueDate;
            context.Response.Info["IdentityDocumentSeries"] = partner.IdentityDocumentSeries;
            context.Response.Info["IdentityDocumentNumber"] = partner.IdentityDocumentNumber;
            context.Response.Info["IdentityDocumentIssuedBy"] = partner.IdentityDocumentIssuedBy;

            context.Response.Info["OGRN"] = partner.OGRN;
        }
    }
}
