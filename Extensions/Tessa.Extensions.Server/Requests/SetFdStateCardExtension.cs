using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.FdProcesses.Shared.Fd.DataHelpers;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class SetFdStateCardExtension : CardRequestExtension
    {
        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Guid mainCardID = context.Request.Info.TryGet<Guid>("mainCardID");
            Guid stateID = context.Request.Info.TryGet<Guid>("stateID");
            string stateName = context.Request.Info.TryGet<string>("stateName");

            await using (context.DbScope.Create())
            {
                FdCardStateDataHelper.ChangeMainCardState(context.DbScope, mainCardID, stateID, stateName);
            }
        }
    }
}
