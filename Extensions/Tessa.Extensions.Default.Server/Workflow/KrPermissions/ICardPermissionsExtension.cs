﻿using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <summary>
    /// Расширение прав на карточку
    /// </summary>
    public interface ICardPermissionsExtension : IExtension
    {
        /// <summary>
        /// Метод, расширяющий права на карточку.
        /// </summary>
        /// <param name="context">
        /// Контекст расширения прав доступа
        /// </param>
        Task ExtendPermissionsAsync(IKrPermissionsManagerContext context);

        /// <summary>
        /// Указывает системе на необходимость пересчета прав при сохранении карточки, а также на получение
        /// контента файла и контента версии, несмотря на то, что при чтении были выданы права на изменение и записаны в токен.
        /// Пример:
        /// Пользователь может редактировать договора с суммой до 100р. Он редактирует карточку с суммой
        /// 80р, при запросе прав система выдала права на редактирование и записала в токен. Это
        /// значит, что пользователь сможет поменять сумму на большую чем 100р. Если такое поведение запрещено
        /// вашей логикой, то расширение должно проверить изменилась ли сумма договора из контекста и вернуть 
        /// true, тогда система пересчитает права перед сохранением и пользователь получит сообщение
        /// о недостаточности прав для изменения карточки.
        /// </summary>
        /// <param name="context">
        /// Контекст расширения проверки правила доступа с возможностью установки флага перерасчета токена прав доступа.
        /// </param>
        Task IsPermissionsRecalcRequired(IKrPermissionsRecalcContext context);
    }
}
