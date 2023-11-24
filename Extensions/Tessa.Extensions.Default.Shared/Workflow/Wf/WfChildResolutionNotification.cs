using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Notices;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.Wf
{
    /// <summary>
    /// Уведомление при завершении дочерней резолюции.
    /// </summary>
    [Notification(Key)]
    public sealed class WfChildResolutionNotification :
        ITaskNotification
    {
        #region Constructors

        public WfChildResolutionNotification(Guid taskID, string result, string body = null)
        {
            this.taskID = taskID;
            this.result = result;
            this.body = body;
        }

        public WfChildResolutionNotification(IDictionary<string, object> storage)
        {
            this.taskID = storage.Get<Guid>(TaskIDKey);
            this.result = storage.TryGet<string>(ResultKey);
            this.body = storage.TryGet<string>(BodyKey);
        }

        #endregion

        #region Private Constants

        private const string TaskIDKey = "TaskID";

        private const string ResultKey = "Result";

        private const string BodyKey = "Body";

        #endregion

        #region Public Constants

        /// <summary>
        /// Ключ, по которому должен быть зарегистрирован обработчик <see cref="INotificationSender"/>.
        /// Ключ также используется для указания секции при сериализации уведомлений.
        /// Ключ должен быть уникальным для каждого типа уведомлений.
        /// </summary>
        public const string Key = "childResolutions";

        #endregion

        #region Properties

        private readonly string result;

        /// <summary>
        /// Результат завершения дочерней резолюции.
        /// </summary>
        public string Result
        {
            get { return this.result; }
        }

        #endregion

        #region ITaskNotification Members

        private readonly Guid taskID;

        /// <summary>
        /// Идентификатор завершённой дочерней резолюции.
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
