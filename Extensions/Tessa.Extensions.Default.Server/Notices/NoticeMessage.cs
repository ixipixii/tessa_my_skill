namespace Tessa.Extensions.Default.Server.Notices
{
    public class NoticeMessage
    {
        #region Properties

        /// <summary>
        /// Отправитель письма (e-mail).
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Тема письма.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Тело письма.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Приложенные к письму файлы.
        /// </summary>
        public NoticeAttachment[] Attachments { get; set; }

        #endregion
    }
}