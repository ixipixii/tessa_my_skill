using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Notices;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.Wf
{
    /// <summary>
    /// Уведомление при отзыве задания в Workflow.
    /// Отсылается пользователю, у которого задание было в работе на момент отзыва,
    /// а также всем его активным заместителям в роли, на которую назначено задание.
    /// </summary>
    [Notification(Key)]
    public sealed class WfRevokeNotification :
        ITaskNotification
    {
        #region Constructors

        public WfRevokeNotification(Guid taskID, Guid userID, string result, string body = null)
        {
            this.taskID = taskID;
            this.userID = userID;
            this.result = result;
            this.body = body;
        }

        public WfRevokeNotification(IDictionary<string, object> storage)
        {
            this.taskID = storage.Get<Guid>(TaskIDKey);
            this.userID = storage.Get<Guid>(UserIDKey);
            this.result = storage.TryGet<string>(ResultKey);
            this.body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string TaskIDKey = "TaskID";

        private const string UserIDKey = "UserID";

        private const string ResultKey = "Result";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "revoked";

        #endregion

        #region Properties

        private readonly Guid userID;

        /// <summary>
        /// Идентификатор пользователя, у которого задание было в работе в момент отзыва.
        /// </summary>
        public Guid UserID
        {
            get { return this.userID; }
        }


        private readonly string result;

        /// <summary>
        /// Результат завершения отозванной резолюции.
        /// Может содержать комментарий автора.
        /// </summary>
        public string Result
        {
            get { return this.result; }
        }

        #endregion

        #region ITaskNotification Members

        private readonly Guid taskID;

        /// <summary>
        /// Идентификатор отозванного задания.
        /// </summary>
        public Guid TaskID
        {
            get { return this.taskID; }
        }

        #endregion

        #region INotification Members

        private readonly string body;

        /// <summary>
        /// Тело письма. Если значение отлично от <c>null</c>,
        /// то оно будет использовано в качестве тела письма,
        /// в противном случае используется стандартное тело.
        /// </summary>
        public string Body
        {
            get { return this.body; }
        }


        public void SerializeTo(IDictionary<string, object> storage)
        {
            storage[TaskIDKey] = this.taskID;
            storage[UserIDKey] = this.userID;

            if (this.result != null)
            {
                storage[ResultKey] = this.result;
            }
            else
            {
                storage.Remove(ResultKey);
            }

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
