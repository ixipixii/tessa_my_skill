using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление о завершении задания доп. согласования.
    /// </summary>
    [Notification(Key)]
    public sealed class AdditionalApprovalNotification :
        INotification
    {
        #region Constructors

        public AdditionalApprovalNotification(
            Guid cardID, 
            Guid userID, 
            string performerName,
            bool result, 
            string comment, 
            int notCompletedTasksCount, string body = null)
        {
            this.cardID = cardID;
            this.userID = userID;
            this.performerName = performerName;
            this.result = result;
            this.comment = comment;
            this.notCompletedTasksCount = notCompletedTasksCount;
            this.body = body;
        }

        public AdditionalApprovalNotification(IDictionary<string, object> storage)
        {
            this.cardID = storage.Get<Guid>(CardIDKey);
            this.userID = storage.Get<Guid>(UserIDKey);
            this.performerName = storage.Get<string>(PerformerNameKey);
            this.result = storage.Get<bool>(ResultKey);
            this.comment = storage.Get<string>(CommentKey);
            this.notCompletedTasksCount = storage.Get<int>(NotCompletedTasksCountKey);
            this.body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string CardIDKey = "CardID";

        private const string UserIDKey = "UserID";

        private const string PerformerNameKey = "PerformerName";

        private const string ResultKey = "Result";

        private const string CommentKey = "Comment";

        private const string NotCompletedTasksCountKey = "NotCompletedTasksCount";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "additionalApproval";

        #endregion

        #region Properties

        private readonly Guid cardID;

        public Guid CardID
        {
            get { return this.cardID; }
        }

        private readonly Guid userID;

        public Guid UserID
        {
            get { return this.userID; }
        }

        private readonly string performerName;

        public string PerformerName
        {
            get { return this.performerName; }
        }

        private readonly bool result;

        public bool Result
        {
            get { return this.result; }
        }

        private readonly string comment;

        public string Comment
        {
            get { return this.comment; }
        }

        private readonly int notCompletedTasksCount;

        public int NotCompletedTasksCount
        {
            get { return this.notCompletedTasksCount; }
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
            storage[UserIDKey] = this.userID;
            storage[PerformerNameKey] = this.performerName;
            storage[ResultKey] = this.result;
            storage[CommentKey] = this.comment;
            storage[NotCompletedTasksCountKey] = this.notCompletedTasksCount;

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
