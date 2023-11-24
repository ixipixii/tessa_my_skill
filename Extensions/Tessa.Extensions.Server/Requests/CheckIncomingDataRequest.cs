using System;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    /// <summary>
    /// Незарегистрированный реквест, решил оставить, возможно потом пригодится для входящих.
    /// </summary>
    public sealed class CheckIncomingDataRequest : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful || context.Request.CardID == null)
            {
                return;
            }
            Guid CorrespondentID = context.Request.Info.TryGet<Guid>("CorrespondentID");
            DateTime ExternalDate = context.Request.Info.TryGet<DateTime>("ExternalDate");
            string ExternalNumber = context.Request.Info.TryGet<string>("ExternalNumber");
            Guid newCardID = (Guid) context.Request.CardID;

            Guid exist = await PnrDataHelper.CheckIncomingDataAsync(
                context.DbScope,
                CorrespondentID,
                ExternalDate,
                ExternalNumber,
                newCardID);
            context.Response.Info.Add("exist", exist);
        }
    }
}
