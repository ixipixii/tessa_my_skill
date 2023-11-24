using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление об отправке задания.
    /// </summary>
    [Notification(Key)]
    public sealed class TaskNotification :
        ITaskNotification
    {
        #region Constructors

        public TaskNotification(Guid taskID, string body = null)
        {
            this.taskID = taskID;
            this.body = body;
        }

        public TaskNotification(IDictionary<string, object> storage)
        {
            this.taskID = storage.Get<Guid>(TaskIDKey);
            this.body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string TaskIDKey = "TaskID";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "tasks";

        #endregion

        #region ITaskNotification Members

        private readonly Guid taskID;

        public Guid TaskID
        {
            get { return this.taskID; }
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
            storage[TaskIDKey] = this.taskID;

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
