using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Chronos.Notices;
using Tessa.Extensions.Default.Server.Notices;

namespace Tessa.Extensions.Default.Chronos.Workflow
{
    public class TestMailReceiver : IMailReceiver
    {
        #region Constructors

        public TestMailReceiver(IMessageProcessor processor)
        {
            this.Processor = processor;
            this.ReceivedMessages = new List<MailSenderMessage>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Объект, выполняющий обработку сообщений.
        /// </summary>
        public IMessageProcessor Processor { get; }

        /// <summary>
        /// Список сообщений, полученных извне.
        /// </summary>
        public List<MailSenderMessage> ReceivedMessages { get; set; }

        #endregion

        #region IMailReceiver Methods

        public Func<bool> StopRequestedFunc { get; set; }


        public async Task ReceiveMessagesAsync(CancellationToken cancellationToken = default)
        {
            // Перебираем сообщения из проперти ReceivedMessages и отдаём их процессору
            foreach (var message in this.ReceivedMessages)
            {
                await this.Processor.ProcessMessageAsync(
                    new NoticeMessage
                    {
                        From = message.Message.Email,
                        Subject = message.Message.Subject,
                        Body = message.Message.Body.Trim(),
                        Attachments = message
                            .MessageFiles
                            .Select(x => new NoticeAttachment() { Name = x.Name, Data = File.ReadAllBytes(x.Path)})
                            .ToArray(),
                    },
                    cancellationToken);
            }
        }

        #endregion
    }
}