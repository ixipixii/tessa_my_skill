using System;

namespace Tessa.Extensions.Default.Shared
{
    /// <summary>
    /// Идентификаторы карточек уведомлений, используемые в типовом решении и в платформе.
    /// </summary>
    public static class DefaultNotifications
    {
        /// <summary>
        /// Идентификатор карточки уведомления для массового ознакомления.
        /// </summary>
        public static readonly Guid AcquaintanceID = new Guid("9e3d20a6-0dff-4667-a29d-30296635c89a");

        /// <summary>
        /// Идентификатор карточки уведомления для предупреждений по сроку истечения пароля.
        /// </summary>
        public static readonly Guid PasswordExpiresID = new Guid("70da1d51-9f21-4693-aebd-0a05e2190027");

        /// <summary>
        /// Уведомление о задании типового решения
        /// </summary>
        public static Guid TaskNotification = new Guid("20411ff1-be39-4bcd-9bd2-d644bf2bb777");

        /// <summary>
        /// Уведомление о завершении задания доп. согласования
        /// </summary>
        public static Guid AdditionalApprovalNotification = new Guid("e0c88f19-bb4a-4d63-b9aa-c41e0533a73a");

        /// <summary>
        /// Уведомление о завершении доп. согласования
        /// </summary>
        public static Guid AdditionalApprovalNotificationCompleted = new Guid("4ef60907-b3c3-4c3b-bc73-ca5603a13b44");

        /// <summary>
        /// Уведомление об ответе на запрос комментария
        /// </summary>
        public static Guid CommentNotification = new Guid("d1c0d80e-f000-4797-9d56-17bee2c133f9");

        /// <summary>
        /// Уведомление о необходимости изменить токен для подписи
        /// </summary>
        public static Guid TokenNotification = new Guid("ff600045-49b1-47d0-a75e-b0e20601d3ae");

        /// <summary>
        /// Уведомление о завершении подзадачи
        /// </summary>
        public static Guid WfChildResolutionNotification = new Guid("ca6fa961-d220-4bf2-8a33-b54f84136b0c");

        /// <summary>
        /// Уведомление об отзыве задачи
        /// </summary>
        public static Guid WfRevokeNotification = new Guid("26b2e1a1-8c57-4028-bb41-1966552656e2");

        /// <summary>
        /// Уведомление о согласовании
        /// </summary>
        public static Guid ApprovedNotification = new Guid("b745d1e2-b1c2-410f-be0b-e0dc9602bbc1");

        /// <summary>
        /// Уведомление о возврате задания из отложенного
        /// </summary>
        public static Guid ReturnFromPostponeNotification = new Guid("93a7fb23-6658-46f9-b9f4-83e1463a123a");

        /// <summary>
        /// Уведомление об отправке ежедневных уведомлений о задании. 
        /// </summary>
        public static Guid TasksNotification = new Guid("8cb57058-cc51-4ce9-a359-9f408f1ae808");

        /// <summary>
        /// Уведомление о новых сообщениях в обсуждениях.
        /// </summary>
        public static Guid ForumNewMessagesNotification = new Guid("39e0f3ea-e71f-494e-937f-17df7d420319");
    }
}
