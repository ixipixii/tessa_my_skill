using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Server.Integration.Helpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Settings;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Senders
{
    public abstract class PnrMdmBaseSender
    {
        protected readonly IDbScope dbScope;
        protected readonly ICardRepository cardRepository;
        protected readonly ICardCache cardCache;
        protected readonly IValidationResultBuilder validationResult;
        protected readonly ILogger logger;
        protected readonly Card card;
        protected readonly ISession session;

        protected PnrMdmBaseSender(IDbScope dbScope, ICardRepository cardRepository, ICardCache cardCache, IValidationResultBuilder validationResult, ILogger logger, Card card, ISession session)
        {
            this.dbScope = dbScope;
            this.cardRepository = cardRepository;
            this.cardCache = cardCache;
            this.validationResult = validationResult;
            this.logger = logger;
            this.card = card;
            this.session = session;
        }

        /// <summary>
        /// Нужно ли отправить карточку в НСИ
        /// </summary>
        protected abstract bool IsNeedSendMessageToMdm(DateTime? lastSentDate);

        /// <summary>
        /// Основная секция карточки
        /// </summary>
        protected abstract string MainSectionName { get; }

        /// <summary>
        /// Преобразование карточки в подходящий для передачи XML объект
        /// </summary>
        /// <returns></returns>
        protected abstract Task<object> GetMessageDataFromCardAsync(Card loadedCard);

        /// <summary>
        /// Получение даты последней отправки в MDM (НСИ)
        /// </summary>
        private async Task<DateTime?> GetContractMDMSentDateAsync()
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var result = await db
                    .SetCommand(
                            $@"SELECT TOP 1 MDMSentDate
                            FROM {MainSectionName} with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", card.ID))
                    .ExecuteAsync<DateTime?>();

                if (result != null)
                {
                    // не забудем указать, что это UTC дата
                    result = DateTime.SpecifyKind(result.Value, DateTimeKind.Utc);
                }

                return result;
            }
        }

        /// <summary>
        /// Обновление даты последней отправки в MDM (НСИ) текущей датой
        /// </summary>
        private async Task<DateTime?> UpdateContractMDMSentDateAsync()
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            $@"UPDATE {MainSectionName}
                            SET MDMSentDate = @sentDate
                            WHERE ID = @ID",
                        db.Parameter("@ID", card.ID),
                        db.Parameter("@sentDate", DateTime.UtcNow))
                    .ExecuteAsync<DateTime?>();
            }
        }

        private async Task<bool> SendMessageDataToMdmHandlerAsync(ICardCache cardCache, ILogger logger, IValidationResultBuilder validationResult, object mdmMessageData)
        {
            // адрес сервиса получим из карточки настроек
            PnrCommonSettings pnrCommonSettings = new PnrCommonSettings(cardCache);
            string mdmServiceUrl = await pnrCommonSettings.GetMdmServiceUrl();

            if (string.IsNullOrWhiteSpace(mdmServiceUrl))
            {
                validationResult.AddError("Не удалось определить адрес сервиса для передачи в НСИ.");
                return false;
            }

            // преобразуем объект в XML текст
            string messageXml = PnrXmlHelper.ToXML(mdmMessageData);

            if (messageXml != null
                && messageXml.StartsWith("<?xml"))
            {
                int messageTagIndex = messageXml.IndexOf("<Message");
                if (messageTagIndex > 0)
                {
                    // заигнорим началный тег <?xml...
                    messageXml = messageXml.Substring(messageTagIndex);
                }
            }

            // Подменим тэги внутри Body
            if (messageXml != null && messageXml.Contains("<Body>"))
            {
                int bodyTagIndexBegin = messageXml.IndexOf("<Body>") + "<Body>".Length;
                int bodyTagIndexEnd = messageXml.IndexOf("</Body>");
                var textNew = messageXml.Substring(bodyTagIndexBegin, bodyTagIndexEnd - bodyTagIndexBegin)
                    .Replace("xsi:nil=\"true\"", "p2:nil=\"true\" xmlns:p2=\"http://www.w3.org/2001/XMLSchema-instance\"");
                //.Replace("<", "&lt;").Replace(">", "&gt;");
                // сконвертируем в формат base64
                textNew = Convert.ToBase64String(Encoding.UTF8.GetBytes(textNew));
                messageXml = messageXml.Remove(bodyTagIndexBegin, bodyTagIndexEnd - bodyTagIndexBegin);
                messageXml = messageXml.Insert(bodyTagIndexBegin, textNew);
            }

            // флаг тестового режима
            bool isTestMode = mdmServiceUrl.Equals("test", StringComparison.OrdinalIgnoreCase);

            // тут будут ошибки отправки в НСИ, если что
            ValidationResultBuilder postXmlValidationResult = new ValidationResultBuilder();

            // не будем обращаться к сервису в тестовом режиме
            bool isSent = isTestMode
                || PnrMdmHttpClientHelper.PostXMLData(logger, postXmlValidationResult, mdmServiceUrl, messageXml, out var postXmlResult);

            if (isSent)
            {
                validationResult.AddInfo($"{(isTestMode ? "[Тестовый режим] " : "")}Карточка успешно отправлена в НСИ.");
                return true;
            }
            else
            {
                validationResult.AddError("При отправке карточки в НСИ произошла ошибка:" + Environment.NewLine + postXmlValidationResult.Build().ToString());
            }
            return false;
        }



        /// <summary>
        /// Если необходимо, отправляет карточку в НСИ
        /// </summary>
        public async Task TrySendCardToMdm(ICardStoreExtensionContext context)
        {
            // определим дату последней отправки
            var lastSentDate = await GetContractMDMSentDateAsync();

            // если с даты последней отправки прошло небольшое количество времени, не будем отправлять карточку
            // т.е. это своего рода защита от потенциальных вложенных сохранений и дублирующих отправок
            // 2 секунд по идее достаточно
            if (lastSentDate != null && lastSentDate.Value.AddSeconds(2) > DateTime.UtcNow)
            {
                return;
            }

            // если бы передан флаг, то точно надо отправить карточку в НСИ
            bool isNeedSendCardToMDM = context.Request.Info.ContainsKey(PnrInfoKeys.PnrIsNeedSendCardToMDM);

            // проверим, нужно ли отправить карточку в НСИ
            bool isNeedSend = isNeedSendCardToMDM || IsNeedSendMessageToMdm(lastSentDate);

            if (isNeedSend)
            {
                // загрузим карточку заново, чтобы отправить её в НСИ
                var loadedCard = await PnrCardHelper.LoadCardAsync(cardRepository, validationResult, card.ID);
                if (loadedCard == null
                    || !validationResult.IsSuccessful())
                {
                    return;
                }

                // преобразуем карточку в объект
                var mdmMessageData = await GetMessageDataFromCardAsync(loadedCard);
                if (mdmMessageData == null
                    || !validationResult.IsSuccessful())
                {
                    return;
                }

                // отправим карточку в НСИ
                var isSent = await SendMessageDataToMdmHandlerAsync(cardCache, logger, validationResult, mdmMessageData);

                // запомним дату отправки
                if (isSent)
                {
                    await UpdateContractMDMSentDateAsync();
                }
            }
        }
    }
}
