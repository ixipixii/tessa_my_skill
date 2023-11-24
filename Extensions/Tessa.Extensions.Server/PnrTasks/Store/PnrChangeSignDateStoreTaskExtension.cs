using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    public sealed class PnrChangeSignDateStoreTaskExtension : CardStoreTaskExtension
    {
        /// <summary>
        /// Сохранение карточки задания и передача комментария в Digest
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StoreTaskAfterRequest(ICardStoreTaskExtensionContext context)
        {
            CardTask task;
            Card card;
            if (!context.RequestIsSuccessful
                || (task = context.Task) == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            Guid? kindID = await GetContractOrSuppKindAsync(context, card);
            
                // Если договор или ДС
            if ((card.TypeID == PnrCardTypes.PnrContractTypeID || 
                 card.TypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID)
                &&
                // и вид ЦФО или ДУП или Внутрихолдинговый 
                 (kindID == PnrContractKinds.PnrContractCFOID || 
                  kindID == PnrContractKinds.PnrContractDUPID ||
                  kindID == PnrContractKinds.PnrContractIntercompanyID)
                &&
                // и этап завершен подписанием документа
                task.OptionID == PnrCompletionOptions.PnrSignDocument)
            {
                Guid cardID = card.ID;
                DateTime? signDate = DateTime.Now;
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    await db.SetCommand(
                            (card.TypeID == PnrCardTypes.PnrContractTypeID ?
                                "UPDATE [dbo].[PnrContracts] " :
                                "UPDATE [dbo].[PnrSupplementaryAgreements]")
                                          + "SET ProjectDate = @signDate "
                                          + "WHERE ID = @cardID;",
                            db.Parameter("cardID", cardID),
                            db.Parameter("signDate", signDate))
                        .LogCommand()
                        .ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// В зависимости от того договор или ДС получаем его тип.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        private async Task<Guid?> GetContractOrSuppKindAsync(ICardStoreTaskExtensionContext context, Card card)
        {
            if (card.TypeID == PnrCardTypes.PnrContractTypeID)
            {
                return await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "KindID");
            }
            return await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrSupplementaryAgreements", "KindID");
        }
    }
}
