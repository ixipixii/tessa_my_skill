using System;

namespace Tessa.Extensions.Default.Shared.Notices
{
    /// <summary>
    /// Уведомление, связанное с заданием с указанным идентификатором.
    /// </summary>
    public interface ITaskNotification :
        INotification
    {
        /// <summary>
        /// Идентификатор задания, с которым связано уведомление.
        /// </summary>
        Guid TaskID { get; }
    }
}
