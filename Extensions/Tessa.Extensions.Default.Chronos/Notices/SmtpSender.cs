using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.IO;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public sealed class SmtpSender : MailSender, IDisposable
    {
        #region Constructors

        public SmtpSender(
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

        #region Fields

        private SmtpClient smtpClient;

        private readonly object syncObject = new object();

        #endregion

        #region Properties

        public string SmtpFrom { get; set; }

        public string SmtpFromDisplayName { get; set; }

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

                var fromAddress = new MailAddress(
                    this.SmtpFrom,
                    string.IsNullOrWhiteSpace(this.SmtpFromDisplayName) ? null : this.SmtpFromDisplayName,
                    Encoding.UTF8);

                string mainRecipientDisplayName = message.Info.MainRecipientDisplayName;
                var toAddress = new MailAddress(
                    message.Message.Email,
                    string.IsNullOrWhiteSpace(mainRecipientDisplayName) ? null : mainRecipientDisplayName,
                    Encoding.UTF8);

                using (var smtpMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = message.Message.Subject,
                    Body = body,
                    IsBodyHtml = message.Info.Format != MailFormat.PlainText,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    BodyTransferEncoding = TransferEncoding.EightBit,
                })
                {
                    ListStorage<MailRecipient> recipients = message.Info.TryGetRecipients();
                    if (recipients != null && recipients.Count > 0)
                    {
                        foreach (MailRecipient recipient in recipients)
                        {
                            string displayName = recipient.DisplayName;

                            var recipientAddress = new MailAddress(
                                recipient.Email,
                                string.IsNullOrWhiteSpace(displayName) ? null : displayName,
                                Encoding.UTF8);

                            switch (recipient.Type)
                            {
                                case MailRecipientType.To:
                                    smtpMessage.To.Add(recipientAddress);
                                    break;

                                case MailRecipientType.Cc:
                                    smtpMessage.CC.Add(recipientAddress);
                                    break;

                                case MailRecipientType.Bcc:
                                    smtpMessage.Bcc.Add(recipientAddress);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(MailRecipientType), recipient.Type, null);
                            }
                        }
                    }

                    foreach (ITempFile file in message.MessageFiles)
                    {
                        try
                        {
                            var data = new Attachment(file.Path);
                            smtpMessage.Attachments.Add(data);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex, LogLevel.Error);
                        }
                    }

                    Logger.Trace("Sending message ID='{0}'", message.Message.ID);
                    await this.smtpClient.SendMailAsync(smtpMessage);
                    Logger.Trace("Message sent. ID='{0}'", message.Message.ID);
                }

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
            if (this.smtpClient == null)
            {
                lock (this.syncObject)
                {
                    if (this.smtpClient == null)
                    {
                        this.smtpClient = CreateSmtpClient();
                    }
                }
            }
        }

        private static SmtpClient CreateSmtpClient()
        {
            // описание SMTP-настроек см. в https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/network/network-element-network-settings

            string pickupDirectoryLocation = NoticeMailerConfig.SmtpPickupDirectoryLocation;
            if (!string.IsNullOrEmpty(pickupDirectoryLocation))
            {
                // складываем письма в папку, которая соответствует либо абсолютному пути, либо пути относительно папки с текущей сборкой
                if (!Path.IsPathRooted(pickupDirectoryLocation))
                {
                    string currentFolder = Assembly.GetExecutingAssembly().GetActualLocationFolder();
                    pickupDirectoryLocation = Path.Combine(currentFolder, pickupDirectoryLocation);
                }

                // если папка уже есть или не удалось её создать - игнорируем ошибки, их будет выбрасывать сам SmtpClient
                FileHelper.CreateDirectoryIfNotExists(pickupDirectoryLocation);

                // указываем хост "localhost", чтобы не было обращений к конфигурации
                return new SmtpClient("localhost")
                {
                    DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                    PickupDirectoryLocation = pickupDirectoryLocation,
                };
            }

            // отправляем письма по SMTP, читаем все настройки из конфига
            string host = NoticeMailerConfig.SmtpHost;
            int port = NoticeMailerConfig.SmtpPort;
            bool enableSsl = NoticeMailerConfig.SmtpEnableSsl;
            bool defaultCredentials = NoticeMailerConfig.SmtpDefaultCredentials;
            string userName = NoticeMailerConfig.SmtpUserName;
            string password = NoticeMailerConfig.SmtpPassword;
            string clientDomain = NoticeMailerConfig.SmtpClientDomain;
            string targetName = NoticeMailerConfig.SmtpTargetName;
            int timeout = NoticeMailerConfig.SmtpTimeout;

            var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = defaultCredentials,
            };

            if (!string.IsNullOrEmpty(userName))
            {
                client.Credentials = string.IsNullOrEmpty(clientDomain)
                    ? new NetworkCredential(userName, password)
                    : new NetworkCredential(userName, password, clientDomain);
            }

            if (!string.IsNullOrEmpty(targetName))
            {
                client.TargetName = targetName;
            }

            if (timeout > 0)
            {
                client.Timeout = timeout;
            }

            return client;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.smtpClient != null)
            {
                this.smtpClient.Dispose();
                this.smtpClient = null;
            }
        }

        #endregion
    }
}
