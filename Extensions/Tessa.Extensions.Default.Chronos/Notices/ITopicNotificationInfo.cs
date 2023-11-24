using System;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public interface ITopicNotificationInfo
    {
        /// <summary>
        /// ID сотрудника.
        /// </summary>
        Guid UserID { get; set; }

        /// <summary>
        /// ID карточки.
        /// </summary>
        Guid CardID { get; set; }

        /// <summary>
        /// ID топика
        /// </summary>
        Guid TopicID { get; set; }

        /// <summary>
        /// Заголовок топика
        /// </summary>
        string TopicTitle { get; set; }

        /// <summary>
        /// Описание топика
        /// </summary>
        string TopicDescription { get; set; }

        /// <summary>
        /// Дата сообщения.
        /// </summary>
        DateTime? MessageDate { get; set; }

        /// <summary>
        /// Полное имя автора.
        /// </summary>
        string AuthorName { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        string HtmlText { get; set; }

        /// <summary>
        /// Ссылка.
        /// </summary>
        string Link { get; set; }

        /// <summary>
        /// Ссылка на ЛК. Может отсутствовать
        /// </summary>
        string WebLink { get; set; }
    }
}