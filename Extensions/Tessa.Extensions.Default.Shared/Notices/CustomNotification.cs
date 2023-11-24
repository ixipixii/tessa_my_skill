using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление, не привязанное к заданию.
    /// </summary>
    [Notification(Key)]
    public sealed class CustomNotification :
        INotification
    {
        #region Constructors

        public CustomNotification(string email, string subject, string body)
        {
            this.email = email;
            this.subject = subject;
            this.body = body;
        }

        public CustomNotification(IDictionary<string, object> storage)
        {
            this.email = storage.Get<string>(EmailKey);
            this.subject = storage.Get<string>(SubjectKey);
            this.body = storage.Get<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string EmailKey = "Email";

        private const string SubjectKey = "Subject";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "custom";

        #endregion

        #region Properties

        private readonly string email;

        public string Email
        {
            get { return this.email; }
        }


        private readonly string subject;

        public string Subject
        {
            get { return this.subject; }
        }

        #endregion

        #region INotification Members

        private readonly string body;

        public string Body
        {
            get { return this.body; }
        }


        public void SerializeTo(IDictionary<string, object> storage)
        {
            storage[EmailKey] = this.email;
            storage[SubjectKey] = this.subject;
            storage[BodyKey] = this.body;
        }

        #endregion
    }
}
