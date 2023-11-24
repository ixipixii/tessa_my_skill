using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Integration.Extensions
{
    /// <summary>
    /// Сбросим дату последней отправки в НСИ
    /// </summary>
    class ClearMDMSentDateNewExtension : CardNewExtension
    {
        public override Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.Card;

            // Договор, ДС, Заявка на КА
            if (card.Sections.TryGetValue("PnrContracts", out var mainSection)
                || card.Sections.TryGetValue("PnrSupplementaryAgreements", out mainSection)
                || card.Sections.TryGetValue("PnrPartnerRequests", out mainSection))
            {
                if (mainSection.Fields.TryGet<DateTime?>("MDMSentDate") != null)
                {
                    // сбросим дату последней отправки в НСИ, иначе документ уйдет в НСИ сразу после сохранения
                    mainSection.Fields["MDMSentDate"] = null;
                }
            }

            return Task.CompletedTask;
        }
    }
}
