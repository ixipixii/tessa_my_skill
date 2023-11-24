using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Satellite
{
    public sealed class KrDialogSatelliteNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(
            ICardNewExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (!context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) is null
                || (sections = card.TryGetSections()) is null)
            {
                return;
            }

            if (sections.TryGetValue(KrConstants.KrDialogSatellite.Name, out var dialogSatelliteSec)
                && context.Request.Info.TryGetValue(CardHelper.MainCardIDKey, out var mcidObj)
                && mcidObj is Guid mcid)
            {
                dialogSatelliteSec.Fields[KrConstants.KrDialogSatellite.MainCardID] = mcid;
                dialogSatelliteSec.Fields[KrConstants.KrDialogSatellite.TypeID] = card.TypeID;
            }
        }
    }
}