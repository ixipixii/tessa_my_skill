using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление о комментарии.
    /// </summary>
    [Notification(Key)]
    public sealed class CommentNotification :
        ITaskNotification
    {
        #region Constructors

        public CommentNotification(Guid taskID, string answer, string body = null)
        {
            this.TaskID = taskID;
            this.Answer = answer;
            this.Body = body;
        }

        public CommentNotification(IDictionary<string, object> storage)
        {
            this.TaskID = storage.Get<Guid>(TaskIDKey);
            this.Answer = storage.TryGet<string>(AnswerKey);
            this.Body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string TaskIDKey = "TaskID";

        private const string AnswerKey = "Answer";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "comments";

        #endregion

        #region Properties

        //Пробрасываем здесь, т.к. на момент завершения задания ответ на комментарий еще не содержится в базе
        public string Answer { get; }

        #endregion

        #region ITaskNotification Members

        public Guid TaskID { get; }

        #endregion

        #region INotification Members

        public string Body { get; }


        public void SerializeTo(IDictionary<string, object> storage)
        {
            storage[TaskIDKey] = this.TaskID;

            if (this.Answer != null)
            {
                storage[AnswerKey] = this.Answer;
            }
            else
            {
                storage.Remove(AnswerKey);
            }

            if (this.Body != null)
            {
                storage[BodyKey] = this.Body;
            }
            else
            {
                storage.Remove(BodyKey);
            }
        }

        #endregion
    }
}
