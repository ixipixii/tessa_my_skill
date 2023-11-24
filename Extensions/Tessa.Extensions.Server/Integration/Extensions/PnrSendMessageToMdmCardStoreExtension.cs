using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Integration.Senders;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Extensions
{
    /// <summary>
    /// Расширение отвечает за отправку карточек в НСИ при сохранении (завершении задания)
    /// Выполняет отправку следующих карточек: Заявка на КА, Договор, ДС
    /// </summary>
    class PnrSendMessageToMdmCardStoreExtension : CardStoreExtension
    {
        private readonly ICardRepository cardRepository;
        private readonly ICardCache cardCache;
        private readonly Logger PartnerRequestSendMessageLogger = LogManager.GetLogger("PartnerRequestSendMessage");
        private readonly Logger ContractSendMessageLogger = LogManager.GetLogger("ContractSendMessage");
        private readonly Logger SuppAgrSendMessageLogger = LogManager.GetLogger("SuppAgrSendMessage");

        public PnrSendMessageToMdmCardStoreExtension(ICardRepository cardRepository, ICardCache cardCache)
        {
            this.cardRepository = cardRepository;
            this.cardCache = cardCache;
        }

        private PnrMdmBaseSender GetSender(ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;
            Guid cardTypeID = card.TypeID;

            // Заявка на КА
            if (cardTypeID == PnrCardTypes.PnrPartnerRequestTypeID)
            {
                return new PnrPartnerRequestMdmSender(context.DbScope, cardRepository, cardCache, context.ValidationResult, PartnerRequestSendMessageLogger, card, context.Session);
            }
            // Договор
            else if (cardTypeID == PnrCardTypes.PnrContractTypeID)
            {
                return new PnrContractMdmSender(context.DbScope, cardRepository, cardCache, context.ValidationResult, ContractSendMessageLogger, card, context.Session);
            }
            // ДС
            else if (cardTypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID)
            {
                return new PnrSuppAgrMdmSender(context.DbScope, cardRepository, cardCache, context.ValidationResult, SuppAgrSendMessageLogger, card, context.Session);
            }
            return null;
        }

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            PnrMdmBaseSender mdmSender = GetSender(context);
            if (mdmSender != null)
            {
                // отправка карточки в НСИ (если выполняются условия отправки)
                // основная логика отправки карточек находится в базовом классе для более удобного поддержания кода
                await mdmSender.TrySendCardToMdm(context);
            }
        }
    }
}
