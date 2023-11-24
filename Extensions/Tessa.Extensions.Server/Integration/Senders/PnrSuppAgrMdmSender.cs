using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Server.Integration.Models;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Senders
{
    /// <summary>
    /// ДС, наследуем от договора, чтобы не дублировать код, т.к. отличий в логике очень мало
    /// </summary>
    public class PnrSuppAgrMdmSender : PnrContractMdmSender
    {
        public PnrSuppAgrMdmSender(IDbScope dbScope, ICardRepository cardRepository, ICardCache cardCache, IValidationResultBuilder validationResult, ILogger logger, Card card, ISession session) : base(dbScope, cardRepository, cardCache, validationResult, logger, card, session)
        {
        }

        protected override string MainSectionName => "PnrSupplementaryAgreements";

        protected override async Task<object> GetMessageDataFromCardAsync(Card loadedCard)
        {
            // возьмем все как в договоре
            var result = await base.GetMessageDataFromCardAsync(loadedCard) as PnrContractRequest;

            if (!validationResult.IsSuccessful() || result == null)
            {
                // если были ошибки, выходим
                return null;
            }

            if (loadedCard == null)
            {
                return null;
            }
            if (!loadedCard.Sections.TryGetValue(MainSectionName, out var mainSection))
            {
                validationResult.AddError($"Не удалось найти секцию \"{MainSectionName}\" для карточки {loadedCard.ID}");
                return null;
            }

            if (mainSection.Fields.TryGet<bool?>("IsUntil2019") == true) return result;
            
            // родительский договор
            Guid? mainContractID = mainSection.Fields.TryGet<Guid?>("MainContractID");

            if (mainContractID == null )
            {
                validationResult.AddError($"Не удалось определить ID родительского договора.");
                return null;
            }

            var resultMain = result.Body.ContractData;

            // дополнительно укажем связанный договор
            resultMain.ИдентификаторРодительскойЗаявкиСЭД = mainContractID.Value.ToString();

            return result;
        }
    }

}
