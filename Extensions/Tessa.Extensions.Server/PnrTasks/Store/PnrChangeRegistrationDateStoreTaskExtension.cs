using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    public sealed class PnrChangeRegistrationDateStoreTaskExtension : CardStoreTaskExtension
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

            async void Update(Guid id, string table, string field)
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    await db.SetCommand("UPDATE [dbo].[" + table + "] " + 
                                        "SET " + field + " = @regDate " + 
                                        "WHERE ID = @cardID;"
                        , db.Parameter("cardID", id)
                        , db.Parameter("regDate", DateTime.Now))
                        .LogCommand()
                        .ExecuteNonQueryAsync();
                }
            }

            // ДОГОВОР УК
            if (card.TypeID == PnrCardTypes.PnrContractUKTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrContractsUK", "RegistrationDate");
            }
            // ДС УК
            else if (card.TypeID == PnrCardTypes.PnrSupplementaryAgreementUKTypeID &&
                     task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrSupplementaryAgreementsUK", "RegistrationDate");
            }
            // ВНД
            if (card.TypeID == PnrCardTypes.PnrRegulationTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrRegulations", "RegistrationDate");
            }
            // СЗ
            if (card.TypeID == PnrCardTypes.PnrServiceNoteTypeID &&
                task.OptionID == PnrCompletionOptions.PnrFormalization)
            {
                Update(card.ID, "PnrServiceNote", "ProjectDate");
            }
            // ДОВЕРЕННОСТЬ
            if (card.TypeID == PnrCardTypes.PnrPowerAttorneyTypeID &&
                task.OptionID == PnrCompletionOptions.PnrFormalization)
            {
                Update(card.ID, "PnrPowerAttorney", "ProjectDate");
            }
            // ВХОДЯЩИЙ
            if (card.TypeID == PnrCardTypes.PnrIncomingTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrIncoming", "RegistrationDate");
            }
            // ИСХОДЯЩИЙ
            if (card.TypeID == PnrCardTypes.PnrOutgoingTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrOutgoing", "RegistrationDate");
            }
            // ИСХОДЯЩИЙ УК
            if (card.TypeID == PnrCardTypes.PnrOutgoingUKTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrOutgoingUK", "RegistrationDate");
            }
            // ВХОДЯЩИЙ УК
            if (card.TypeID == PnrCardTypes.PnrIncomingUKTypeID &&
                task.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                Update(card.ID, "PnrIncomingUK", "RegistrationDate");
            }
        }
    }
}
