using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <summary>
    /// Результат выполнения проверки прав доступа в <see cref="IKrPermissionsManager"/>
    /// </summary>
    public interface IKrPermissionsManagerResult
    {
        /// <summary>
        /// Версия правил доступа
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Определяет, были ли запрошены права на редактирование.
        /// </summary>
        bool WithEdit { get; }

        /// <summary>
        /// Набор рассчитанных прав
        /// </summary>
        ICollection<KrPermissionFlagDescriptor> Permissions { get; }

        /// <summary>
        /// Набор прав доступа к сециям карточки
        /// </summary>
        HashSet<Guid, KrPermissionSectionSettings> ExtendedCardSettings { get; }

        /// <summary>
        /// Набор прав доступа к сециям заданий, разбитых по типам заданий
        /// </summary>
        Dictionary<Guid, HashSet<Guid, KrPermissionSectionSettings>> ExtendedTasksSettings { get; }

        /// <summary>
        /// Набор правил доступа для файлов
        /// </summary>
        ICollection<KrPermissionFileRule> FileRules { get; }

        /// <summary>
        /// Определяет, что в результате есть данный флаг
        /// </summary>
        /// <param name="krPermission">Проверяемый флаг доступа. Разворачивает виртуальные флаги</param>
        /// <returns>Возвращает true, если доступ есть, иначе false</returns>
        bool Has(KrPermissionFlagDescriptor krPermission);

        /// <summary>
        /// Создает расширенные настройки прав карточки по результату расчета прав доступа.
        /// Если не при расчете прав не использовались расширенные настройки проверки прав доступа, то метод вернет null
        /// </summary>
        /// <param name="userID">Идентификатор сотрудника, для которого создаются расширеныне настройки доступа</param>
        /// <param name="card">Карточка, для которой создаются расширеныне настройки доступа</param>
        /// <returns>
        /// Возвращает расширенные настройки прав доступа по карточки или null, если при расчете прав доступа не запрашивались расширенные настройки прав доступа
        /// </returns>
        IKrPermissionExtendedCardSettings CreateExtendedCardSettings(Guid userID, Card card);
    }
}
