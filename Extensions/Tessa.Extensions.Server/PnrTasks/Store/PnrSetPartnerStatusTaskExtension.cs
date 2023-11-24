using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    /// <summary>
    /// Установка статуса контрагента по решению СБ
    /// </summary>
    public sealed class PnrSetPartnerStatusTaskExtension : CardStoreTaskExtension
    {
        public override async Task StoreTaskBeforeCommitTransaction(ICardStoreTaskExtensionContext context)
        {
            CardTask tasks;
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (tasks = context.Task) == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            if (card.TypeID == PnrCardTypes.PnrPartnerRequestTypeID
                && (tasks.OptionID == PnrCompletionOptions.PnrBlackList
                    || tasks.OptionID == PnrCompletionOptions.PnrAgreed))
            {
                await SetPartnerStatus(context, tasks, card);

                // отправим карточку в НСИ
                // для этого протолкнем в реквесте спец. флаг
                context.StoreContext.Request.Info[PnrInfoKeys.PnrIsNeedSendCardToMDM] = true;
            }
        }

        /// <summary>
        /// Установка статуса контрагента "Согласован"/"В черном списке"
        /// </summary>
        public async Task SetPartnerStatus(ICardStoreTaskExtensionContext context, CardTask tasks, Card card)
        {
            Guid partnerRequestsID = context.Request.Card.ID;
            int? statusID = null;
            string statusName = null;
            DateTime? dateApproval = null;
            DateTime? validity = null;
            if (tasks.OptionID == PnrCompletionOptions.PnrBlackList)
            {
                statusID = PnrPartnersStatus.BlacklistedID;
                statusName = PnrPartnersStatus.BlacklistedName;
            }
            else if(tasks.OptionID == PnrCompletionOptions.PnrAgreed)
            {
                statusID = PnrPartnersStatus.AgreedID;
                statusName = PnrPartnersStatus.AgreedName;
                dateApproval = DateTime.Now;
                validity = DateTime.Now.AddMonths(6);
            }
                
            if (statusID != null)
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    await db.SetCommand(
                        "UPDATE [dbo].[Partners] "
                        + "SET StatusID = @statusID, "
                        + "StatusName = @statusName, "
                        + "DateApproval = @dateApproval, "
                        + "Validity = @validity "
                        + "WHERE ID = "
                            + "(SELECT p.ID "
                            + "FROM[dbo].[Partners] as p WITH(NOLOCK) "
                            + "LEFT JOIN[dbo].[PnrPartnerRequests] as r WITH(NOLOCK) ON r.ID = @partnerRequestsID "
                            + "WHERE p.ID = r.PartnerID);",
                    db.Parameter("partnerRequestsID", partnerRequestsID),
                    db.Parameter("statusID", statusID),
                    db.Parameter("statusName", statusName),
                    db.Parameter("dateApproval", dateApproval),
                    db.Parameter("validity", validity))
                    .LogCommand()
                    .ExecuteNonQueryAsync();
                }
            }
        }
    }
}
