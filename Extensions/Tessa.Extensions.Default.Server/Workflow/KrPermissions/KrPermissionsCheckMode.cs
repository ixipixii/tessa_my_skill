﻿namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <summary>
    /// Список режимов проверки прав доступа. Определяет методы проверки прав доступа в <see cref="IKrPermissionsManager"/>
    /// </summary>
    public enum KrPermissionsCheckMode
    {
        /// <summary>
        /// При данном событии объект карточки еще не существует, поэтому не выполняется проверка таблицы условий (т.к. она выполняется в контексте карточки) и контекстных ролей.
        /// Применяется при создании карточки.
        /// </summary>
        WithoutCard,

        /// <summary>
        /// При данном событии карточка не передается и загружается из базы.
        /// Применяется при чтении файла, версии файла, удалении карточкию
        /// </summary>
        WithCardID,

        /// <summary>
        /// При данном событии карточка парадется с неполными данными и подразумевается, что актуальные данные карточки находятся в базе.
        /// Применяется при сохранении карточки.
        /// </summary>
        WithStoreCard,

        /// <summary>
        /// При данном событии все данные о карточки уже загружены и актуальны. Применяется при чтении карточки.
        /// </summary>
        WithCard,
    }
}
