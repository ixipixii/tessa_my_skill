using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.PnrCards
{
    class PnrPartnerGetExtension : CardGetExtension
    {
        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || !context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var card = context.Response.Card;

            // Заполним вирт. таблицу связанных заявок 
            await LoadRefRequestsVirtual(context.DbScope, card);
        }

        private async Task LoadRefRequestsVirtual(IDbScope dbScope, Card card)
        {
            if (!card.Sections.TryGetValue("PnrPartner_RefRequestsVirtual", out var rrvSection))
            {
                return;
            }
            if (!card.Sections.TryGetValue("PnrPartner_RefCreateRequestsVirtual", out var rcrvSection))
            {
                return;
            }

            // загрузим связанные заявки
            var partnerRequests = await PnrPartnerRequestHelper.GetPartnerRequestsByPartnerID(dbScope, card.ID);

            foreach (var req in partnerRequests)
            {
                if (req.RequestTypeID == PnrPartnerRequestsTypes.ApproveID && req.StateName != "Проект")
                {
                    var newRow = rrvSection.Rows.Add();
                    newRow.RowID = req.ID;
                    newRow.Fields["DocID"] = req.ID;
                    newRow.Fields["DocFullNumber"] = req.FullNumber;
                    newRow.Fields["CreateDate"] = req.CreationDate;
                    newRow.Fields["RequestType"] = req.RequestTypeName;
                    newRow.Fields["State"] = req.StateName;
                    newRow.State = CardRowState.None;
                }
            }

            foreach (var req in partnerRequests)
            {
                if (req.RequestTypeID == PnrPartnerRequestsTypes.CreateID && req.StateName != "Проект")
                {
                    var newRow = rcrvSection.Rows.Add();
                    newRow.RowID = req.ID;
                    newRow.Fields["DocID"] = req.ID;
                    newRow.Fields["DocFullNumber"] = req.FullNumber;
                }
            }            
        } 
    }
}
