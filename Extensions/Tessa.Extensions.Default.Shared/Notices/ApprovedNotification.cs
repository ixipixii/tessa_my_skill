using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление о завершении согласования.
    /// </summary>
    [Notification(Key)]
    public sealed class ApprovedNotification :
        INotification
    {
        #region Constructors

        public ApprovedNotification(Guid cardID, string body = null)
        {
            this.cardID = cardID;
            this.body = body;
        }

        public ApprovedNotification(IDictionary<string, object> storage)
        {
            this.cardID = storage.Get<Guid>(CardIDKey);
            this.body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string CardIDKey = "CardID";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "approved";

        #endregion

        #region Properties

        private readonly Guid cardID;

        public Guid CardID
        {
            get { return this.cardID; }
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
            storage[CardIDKey] = this.cardID;

            if (this.body != null)
            {
                storage[BodyKey] = this.body;
            }
            else
            {
                storage.Remove(BodyKey);
            }
        }

        #endregion
    }
}
