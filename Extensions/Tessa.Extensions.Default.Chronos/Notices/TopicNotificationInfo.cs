using System;
using System.Collections.Generic;
using System.Globalization;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public class TopicNotificationInfo : StorageObject, ITopicNotificationInfo
    {
        #region Constructors

        public TopicNotificationInfo()
            : base(new Dictionary<string, object>(StringComparer.Ordinal))
        {
            this.Init(nameof(CardID), GuidBoxes.Empty);
            this.Init(nameof(UserID), GuidBoxes.Empty);
            this.Init(nameof(TopicID), GuidBoxes.Empty);
            this.Init(nameof(TopicTitle), null);
            this.Init(nameof(TopicDescription), null);
            this.Init(nameof(MessageDate), null);
            this.Init(nameof(AuthorName), null);
            this.Init(nameof(HtmlText), null);
            this.Init(nameof(Link), null);
            this.Init(nameof(WebLink), null);
        }

        #endregion

        #region ITaskNotificationInfo Members

        public CultureInfo Culture { get; set; }

        /// <summary>
        /// ID карточки.
        /// </summary>
        public Guid CardID
        {
            get { return this.Get<Guid>(nameof(CardID)); }
            set { this.Set(nameof(CardID), value); }
        }

        /// <summary>
        /// ID аотрудника.
        /// </summary>
        public Guid UserID
        {
            get { return this.Get<Guid>(nameof(UserID)); }
            set { this.Set(nameof(UserID), value); }
        }

        /// <summary>
        /// ID топика.
        /// </summary>
        public Guid TopicID
        {
            get { return this.Get<Guid>(nameof(TopicID)); }
            set { this.Set(nameof(TopicID), value); }
        }

        /// <summary>
        /// Заголовок топика
        /// </summary>
        public string TopicTitle
        {
            get { return this.Get<string>(nameof(TopicTitle)); }
            set { this.Set(nameof(TopicTitle), value); }
        }

        /// <summary>
        /// Описание топика
        /// </summary>
        public string TopicDescription
        {
            get { return this.Get<string>(nameof(TopicDescription)); }
            set { this.Set(nameof(TopicDescription), value); }
        }

        /// <summary>
        /// Дата сообщения.
        /// </summary>
        public DateTime? MessageDate
        {
            get { return this.Get<DateTime?>(nameof(MessageDate)); }
            set { this.Set(nameof(MessageDate), value); }
        }

        /// <summary>
        /// Полное имя автора.
        /// </summary>
        public string AuthorName
        {
            get { return this.Get<string>(nameof(AuthorName)); }
            set { this.Set(nameof(AuthorName), value); }
        }


        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string HtmlText
        {
            get { return this.Get<string>(nameof(HtmlText)); }
            set { this.Set(nameof(HtmlText), value); }
        }

        /// <summary>
        /// Ссылка.
        /// </summary>
        public string Link
        {
            get { return this.Get<string>(nameof(Link)); }
            set { this.Set(nameof(Link), value); }
        }

        /// <summary>
        /// Ссылка на веб-клиент.
        /// </summary>
        public string WebLink
        {
            get { return this.Get<string>(nameof(WebLink)); }
            set { this.Set(nameof(WebLink), value); }
        }

        #endregion
    }
}
