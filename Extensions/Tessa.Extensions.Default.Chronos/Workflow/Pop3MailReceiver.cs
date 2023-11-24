using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.Extensions.Default.Chronos.Notices;
using Tessa.Extensions.Default.Server.Notices;
using Tessa.Platform;
using Tessa.Platform.OpenPop.Mime;
using Tessa.Platform.OpenPop.Pop3;

namespace Tessa.Extensions.Default.Chronos.Workflow
{
    public class Pop3MailReceiver : IMailReceiver
    {
        #region Fields

        private readonly Func<IMessageProcessor> getProcessorFunc;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public Pop3MailReceiver(Func<IMessageProcessor> getProcessorFunc)
        {
            // получаем функцию Func<IMessageProcessor>, чтобы зависимости Unity (всякие кэши метаинфы)
            // не инициализировались раньше времени
            this.getProcessorFunc = getProcessorFunc;
        }

        #endregion

        #region Properties

        public bool StopRequested => this.StopRequestedFunc != null && this.StopRequestedFunc();

        public Pop3ServiceSettings Settings { get; set; }

        #endregion

        #region IMailReceiver Methods

        public Func<bool> StopRequestedFunc { get; set; }


        public async Task ReceiveMessagesAsync(CancellationToken cancellationToken = default)
        {
            logger.Trace("Loading messages.");

            string subject = string.Empty;

            using var client = new Pop3Client();
            // ReSharper disable PossibleInvalidOperationException
            client.Connect(this.Settings.Pop3Hostname, (int) this.Settings.Pop3Port, this.Settings.Pop3UseSsl ?? false);
            // ReSharper restore PossibleInvalidOperationException

            // Authenticate ourselves towards the server
            client.Authenticate(this.Settings.Pop3User, this.Settings.Pop3Password);

            if (this.StopRequested)
            {
                return;
            }

            // Get the number of messages in the inbox
            int messageCount = client.GetMessageCount();

            if (messageCount == 0)
            {
                logger.Trace("There are no messages to process.");
                return;
            }

            // Messages are numbered in the interval: [1, messageCount]
            // Ergo: message numbers are 1-based.
            // Most servers give the latest message the highest number
            IMessageProcessor processor = null;
            for (int i = 1; i <= messageCount; i++)
            {
                if (this.StopRequested)
                {
                    return;
                }

                Message message = client.GetMessage(i);
                logger.Trace("Message loaded. From: {0}, Subject: {1}.",
                    message.Headers.From.MailAddress.Address, message.Headers.Subject);

                if (this.StopRequested)
                {
                    return;
                }

                try
                {
                    subject = message.Headers.Subject ?? string.Empty;

                    string messageText = null;

                    MessagePart plainText = message.FindFirstPlainTextVersion();
                    if (plainText != null)
                    {
                        messageText = plainText.GetBodyAsText();
                    }
                    else
                    {
                        MessagePart htmlText = message.FindFirstHtmlVersion();
                        if (htmlText != null)
                        {
                            string html = htmlText.GetBodyAsText();
                            messageText = FormattingHelper.ExtractPlainTextFromHtml(html);
                        }
                    }

                    string from = message.Headers.From.HasValidMailAddress
                        ? message.Headers.From.MailAddress.Address
                        : message.Headers.From.Address;

                    logger.Trace("Finding user and setting user session.");

                    if (processor == null)
                    {
                        processor = this.getProcessorFunc();
                    }

                    await processor.ProcessMessageAsync(
                        new NoticeMessage
                        {
                            From = @from,
                            Subject = subject,
                            Body = messageText ?? string.Empty,
                            Attachments = message
                                .FindAllAttachments()
                                .Select(x => new NoticeAttachment() { Name = x.FileName, Data = x.Body })
                                .ToArray(),
                        },
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }
                finally
                {
                    client.DeleteMessage(i);
                    logger.Trace("Message is deleted. Subject: '{0}'", subject);
                }
            }
        }

        #endregion
    }
}