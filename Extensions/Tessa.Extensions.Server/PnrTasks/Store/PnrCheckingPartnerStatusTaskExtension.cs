using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    /// <summary>
    /// Класс проверки статуса контрагента при запуске процесса по договору
    /// </summary>
    public sealed class PnrCheckingPartnerStatusTaskExtension : CardStoreTaskExtension
    {
        // данные по контрагенту
        private class PartnerInfo
        {
            public int? StatusID { get; set; }
            public string Name { get; set; }
        }

        public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            CardTask tasks;
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (tasks = context.Task) == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            if((card.TypeID == PnrCardTypes.PnrContractTypeID || card.TypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID ||
                card.TypeID == PnrCardTypes.PnrPowerAttorneyTypeID || card.TypeID == PnrCardTypes.PnrTenderTypeID ||
                card.TypeID == PnrCardTypes.PnrPartnerRequestTypeID || card.TypeID == PnrCardTypes.PnrOutgoingUKTypeID ||
                card.TypeID == PnrCardTypes.PnrContractUKTypeID || card.TypeID == PnrCardTypes.PnrSupplementaryAgreementUKTypeID)
                && tasks.OptionID == PnrCompletionOptions.FdStartProcess)
                await CheckingPartnerStatus(context, tasks, card);
        }

        /// <summary>
        /// Проверка статуса КА документа на "Не согласован" и "В черном списке"
        /// </summary>
        public async Task CheckingPartnerStatus(ICardStoreTaskExtensionContext context, CardTask tasks, Card card)
        {
            var partnerInfoList = new List<PartnerInfo>();
            string requestTextPrefix = "SELECT StatusID, p.Name FROM [dbo].[Partners] as p WITH(NOLOCK) ";
            string requestTextSuffix = "";

            switch (card.TypeID)
            {
                case Guid standard when standard == new Guid(PnrCardTypes.PnrIncomingTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrIncoming] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.CorrespondentID;";    // Корреспондент (организация)
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrOutgoingTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrOutgoing] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.DestinationID;";      // Адресат (контрагент)
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrContractTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrContracts] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Контрагент
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrSupplementaryAgreementTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrSupplementaryAgreements] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Контрагент
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrPowerAttorneyTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrPowerAttorney] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.DestinationID;";         // Адресат (организация)
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrTenderTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrTenderBidders] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Адресат (организация)
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrPartnerRequestTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrPartnerRequests] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Контрагент
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrOutgoingUKTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrOutgoingUK] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.DestinationID;";         // Адресат
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrContractUKTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrContractsUK] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Контрагент
                    break;
                case Guid standard when standard == new Guid(PnrCardTypes.PnrSupplementaryAgreementUKTypeID.ToString()):
                    requestTextSuffix = "LEFT JOIN [dbo].[PnrSupplementaryAgreementsUK] as c WITH(NOLOCK) ON c.ID = @cardID WHERE p.ID = c.PartnerID;";         // Контрагент
                    break;
            }


            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                DbManager dbManager = db.SetCommand(
                    requestTextPrefix
                    + requestTextSuffix,
                db.Parameter("cardID", card.ID))
                .LogCommand();

                partnerInfoList = await dbManager.ExecuteListAsync<PartnerInfo>();
            }

            if(partnerInfoList.Count > 0)
            {
                if ((card.TypeID == PnrCardTypes.PnrContractTypeID || 
                    card.TypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID ||
                    card.TypeID == PnrCardTypes.PnrContractUKTypeID ||
                    card.TypeID == PnrCardTypes.PnrSupplementaryAgreementUKTypeID) &&
                    partnerInfoList[0].StatusID != null &&
                    partnerInfoList[0].StatusID == 1)
                {
                    // На карточках Договор, ДС, Договор УК, ДС УК отдельно обрабатываем случай, когда КА 'Не согласован'
                    context.ValidationResult.AddError($"Контрагент документа '{partnerInfoList[0].Name}' не согласован. Необходимо согласовать контрагента в системе.");
                }
                else if (card.TypeID == PnrCardTypes.PnrTenderTypeID)
                {
                    // В Тендере список Участников тендера, проверка на 'В черном списке' каждого участника тендера
                    int inBlackListCount = 0;
                    string inBlackListPartnersNames = "";
                    foreach (PartnerInfo partnerInfo in partnerInfoList)
                    {
                        if (partnerInfo.StatusID != null && partnerInfo.StatusID == 2)
                        {
                            inBlackListCount++;
                            if (String.IsNullOrEmpty(inBlackListPartnersNames))
                                inBlackListPartnersNames += $"'{partnerInfo.Name}'";
                            else inBlackListPartnersNames += $" , '{partnerInfo.Name}'";
                        }
                    }
                    if (inBlackListCount > 0)
                    {
                        string errorMessagePrefix = "Контрагент" + (inBlackListCount > 1 ? "ы" : "");
                        context.ValidationResult.AddError($"{errorMessagePrefix} документа {inBlackListPartnersNames} со статусом 'В черном списке'. Нельзя отправлять на согласование документы с контрагентом, находящимся в указанном статусе. Запуск процесса прерван.");
                    }
                }
                else if (card.TypeID != PnrCardTypes.PnrTenderTypeID && partnerInfoList[0].StatusID != null && partnerInfoList[0].StatusID == 2)
                {
                    // Все оставшиеся карточки с одним КА (КА Договора также будет проверен на 'В черном списке')
                    context.ValidationResult.AddError($"Контрагент документа '{partnerInfoList[0].Name}' со статусом 'В черном списке'. Нельзя отправлять на согласование документы с контрагентом, находящимся в указанном статусе. Запуск процесса прерван.");
                }
            }
            
        }
    }
}
