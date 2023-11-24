using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.IO;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public sealed class ExchangeSender : MailSender
    {
        #region Constructors

        public ExchangeSender(
            ICardServerPermissionsProvider permissionsProvider,
            ICardContentStrategy contentStrategy,
            ICardStreamClientRepository clientRepository,
            ICardRepository extendedRepository,
            IDbScope dbScope,
            IOutboxManager outboxManager)
            : base(permissionsProvider, contentStrategy, clientRepository, extendedRepository, dbScope, outboxManager)
        {
        }

        #endregion

        #region Properties

        public ExchangeServiceSettings Settings
        {
            get { return this.settings; }
            set
            {
                if (this.settings != value)
                {
                    this.settings = value;
                    this.exchangeService = null;
                }
            }
        }

        #endregion

        #region Fields

        private ExchangeService exchangeService;

        private ExchangeServiceSettings settings;

        private readonly object syncObject = new object();

        #endregion

        #region Base Overrides

        protected override async Task<bool> SendMessageAsync(MailSenderMessage message, CancellationToken cancellationToken = default)
        {
            this.EnsureInitialized();

            try
            {
                string body = message.Message.Body;

                this.AddMissedFilesInfo(ref body, message.MissedFiles, MissedFilesRegex);
                this.AddMissedFilesInfo(ref body, message.OversizedFiles, OversizedFilesRegex);

                var exchangeMessage = new EmailMessage(this.exchangeService)
                {
                    Subject = message.Message.Subject,
                    Body = new MessageBody(message.Info.Format == MailFormat.PlainText ? BodyType.Text : BodyType.HTML, body),
                };

                string mainRecipientDisplayName = message.Info.MainRecipientDisplayName;

                var toAddress = string.IsNullOrWhiteSpace(mainRecipientDisplayName)
                    ? new EmailAddress(message.Message.Email)
                    : new EmailAddress(mainRecipientDisplayName, message.Message.Email);

                exchangeMessage.ToRecipients.Add(toAddress);

                ListStorage<MailRecipient> recipients = message.Info.TryGetRecipients();
                if (recipients != null && recipients.Count > 0)
                {
                    foreach (MailRecipient recipient in recipients)
                    {
                        string displayName = recipient.DisplayName;

                        var recipientAddress = string.IsNullOrWhiteSpace(displayName)
                            ? new EmailAddress(recipient.Email)
                            : new EmailAddress(displayName, recipient.Email);

                        switch (recipient.Type)
                        {
                            case MailRecipientType.To:
                                exchangeMessage.ToRecipients.Add(recipientAddress);
                                break;

                            case MailRecipientType.Cc:
                                exchangeMessage.CcRecipients.Add(recipientAddress);
                                break;

                            case MailRecipientType.Bcc:
                                exchangeMessage.BccRecipients.Add(recipientAddress);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(MailRecipientType));
                        }
                    }
                }

                foreach (ITempFile file in message.MessageFiles)
                {
                    try
                    {
                        exchangeMessage.Attachments.AddFileAttachment(file.Path);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }

                Logger.Trace("Sending message ID='{0}'", message.Message.ID);
                await exchangeMessage.Send();
                Logger.Trace("Message sent. ID='{0}'", message.Message.ID);

                return true;
            }
            catch (Exception ex)
            {
                await this.LogProcessingErrorAsync(message.Message, ex, cancellationToken);
                return false;
            }
        }

        #endregion

        #region Methods

        public void EnsureInitialized()
        {
            if (this.settings == null)
            {
                throw new InvalidOperationException("Can't initialize exchange sender without its settings.");
            }

            if (this.exchangeService == null)
            {
                lock (this.syncObject)
                {
                    if (this.exchangeService == null)
                    {
                        this.exchangeService = ExchangeServiceHelper.CreateExchangeService(this.settings);
                    }
                }
            }
        }

        #endregion
    }
}