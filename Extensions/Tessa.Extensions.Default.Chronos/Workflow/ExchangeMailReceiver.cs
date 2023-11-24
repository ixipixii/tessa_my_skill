using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.WebServices.Data;
using NLog;
using Tessa.Extensions.Default.Chronos.Notices;
using Tessa.Extensions.Default.Server.Notices;
using Tessa.Platform;
using Task = System.Threading.Tasks.Task;

namespace Tessa.Extensions.Default.Chronos.Workflow
{
    public class ExchangeMailReceiver : IMailReceiver
    {
        #region Fields

        private readonly Func<IMessageProcessor> getProcessorFunc;

        private ExchangeService service;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public ExchangeMailReceiver(Func<IMessageProcessor> getProcessorFunc)
        {
            // получаем функцию Func<IMessageProcessor>, чтобы зависимости Unity (всякие кэши метаинфы)
            // не инициализировались раньше времени
            this.getProcessorFunc = getProcessorFunc;
        }

        #endregion

        #region Properties

        public bool StopRequested => this.StopRequestedFunc != null && this.StopRequestedFunc();


        private ExchangeServiceSettings settings;

        public ExchangeServiceSettings Settings
        {
            get { return this.settings; }
            set
            {
                this.settings = value;
                this.service = null;
            }
        }

        #endregion

        #region Private Methods

        private static string ExtractPlainText(MessageBody body)
        {
            switch (body.BodyType)
            {
                case BodyType.HTML:
                    return FormattingHelper.ExtractPlainTextFromHtml(body.Text);

                case BodyType.Text:
                    return body.Text;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region IMailReceiver Methods

        public Func<bool> StopRequestedFunc { get; set; }


        public async Task ReceiveMessagesAsync(CancellationToken cancellationToken = default)
        {
            if (this.service is null)
            {
                logger.Trace("Initializing exchange service.");
                this.service = ExchangeServiceHelper.CreateExchangeService(this.settings);
            }

            Folder incomingFolder = await Folder.Bind(this.service, WellKnownFolderName.Inbox);

            if (incomingFolder is null || incomingFolder.TotalCount == 0)
            {
                logger.Trace("There are no messages to process.");
                return;
            }

            if (this.StopRequested)
            {
                return;
            }

            ItemView itemView = new ItemView(incomingFolder.TotalCount);
            itemView.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            FindItemsResults<Item> items = await incomingFolder.FindItems(itemView);

            if (this.StopRequested)
            {
                return;
            }

            PropertySet itemPropertySet = new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Attachments);

            bool askTextBody = this.service.RequestedServerVersion >= ExchangeVersion.Exchange2013;
            if (askTextBody)
            {
                itemPropertySet.Add(ItemSchema.TextBody);
            }

            itemPropertySet.RequestedBodyType = BodyType.Text;
            itemView.PropertySet = itemPropertySet;

            IMessageProcessor processor = null;
            foreach (Item item in items)
            {
                if (this.StopRequested)
                {
                    return;
                }

                logger.Trace("Message loading. Subject: '{0}'.", item.Subject);
                var message = item as EmailMessage;

                try
                {
                    if (message != null)
                    {
                        await message.Load(itemPropertySet);
                        logger.Trace("Message loaded. From: '{0}', Subject: '{1}'.", message.From, message.Subject);

                        logger.Trace("Finding user and setting user session.");

                        string body;
                        if (askTextBody)
                        {
                            try
                            {
                                body = message.TextBody.Text.Trim();
                            }
                            catch (Exception ex)
                            {
                                logger.LogException(ex, LogLevel.Warn);
                                body = ExtractPlainText(message.Body);
                            }
                        }
                        else
                        {
                            body = ExtractPlainText(message.Body);
                        }

                        var attachments = new List<NoticeAttachment>();

                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is FileAttachment fileAttachment)
                            {
                                await fileAttachment.Load();

                                attachments.Add(
                                    new NoticeAttachment
                                    {
                                        Name = string.IsNullOrEmpty(fileAttachment.FileName)
                                            ? fileAttachment.Name
                                            : fileAttachment.FileName,
                                        Data = fileAttachment.Content,
                                    });
                            }
                        }

                        if (processor is null)
                        {
                            processor = this.getProcessorFunc();
                        }

                        await processor.ProcessMessageAsync(
                            new NoticeMessage
                            {
                                From = message.From.Address,
                                Subject = message.Subject,
                                Body = body,
                                Attachments = attachments.ToArray(),
                            },
                            cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }
                finally
                {
                    if (message != null)
                    {
                        await message.Delete(DeleteMode.HardDelete);
                        logger.Trace("Message is deleted. From: '{0}', Subject: '{1}'", message.From.Address, message.Subject);
                    }
                }
            }
        }

        #endregion
    }
}