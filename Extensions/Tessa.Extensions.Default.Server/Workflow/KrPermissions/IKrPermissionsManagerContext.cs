using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <summary>
    /// Объект контекста менеджера проверки прав доступа <see cref="IKrPermissionsManager"/>
    /// </summary>
    public interface IKrPermissionsManagerContext : IExtensionContext
    {
        /// <summary>
        /// Контекст расширения, в котором была вызвана проверка прав доступа.
        /// Может быть равна null.
        /// </summary>
        ICardExtensionContext ExtensionContext { get; }
        
        /// <summary>
        /// Предыдущий токен прав доступа. Может быть не задан.
        /// </summary>
        KrToken PreviousToken { get; }

        /// <summary>
        /// Дополнительный токен прав доступа, рассчитанный на сервере.
        /// Его настройки приоритетнее, чем в <see cref="PreviousToken"/> и он всегда считается валидным.
        /// Может быть не задан.
        /// </summary>
        KrToken ServerToken { get; }

        /// <summary>
        /// Дескриптор с проверкой парил доступа
        /// </summary>
        KrPermissionsDescriptor Descriptor { get; set; }

        /// <summary>
        /// Режим проверки доступа к карточке
        /// </summary>
        KrPermissionsCheckMode Mode { get; }

        /// <summary>
        /// Определяет имя метода, который был вызван.
        /// Может иметь значение <see cref="IKrPermissionsManager.CheckRequiredPermissionsAsync"/> или <see cref="IKrPermissionsManager.GetEffectivePermissionsAsync"/>
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// Карточка, по которой идет проверка доступа. 
        /// Ее наличие и содержимое зависит от <see cref="Mode"/>
        /// </summary>
        Card Card { get; }

        /// <summary>
        /// Идентификатор карточки или null, если проверка идет вне контекста карточки
        /// </summary>
        Guid? CardID { get; }

        /// <summary>
        /// Идентификатор типа карточки
        /// </summary>
        CardType CardType { get; }

        /// <summary>
        /// Идентификатор типа документа, если используется тип документа, иначе null
        /// </summary>
        Guid? DocTypeID { get; }

        /// <summary>
        /// Состояние карточки
        /// </summary>
        KrState? DocState { get; }

        /// <summary>
        /// Идентификатор файла, если идет проверка доступа к файлу
        /// </summary>
        Guid? FileID { get; }

        /// <summary>
        /// Идентификатор версии файла, если идет проверка доступа к конкретной версии файла
        /// </summary>
        Guid? FileVersionID { get; }

        /// <summary>
        /// Флаг определяет, что флаги правил доступа, у которых стоит флаг <see cref="IKrPermissionRuleSettings.IsRequired"/>,
        /// добавляются к списку запрашиваемых правил доступа, если правло прошло.
        /// </summary>
        bool WithRequiredPermissions { get; }

        /// <summary>
        /// Флаг определяет, что нужно рассчитать расширенные права доступа.
        /// </summary>
        bool WithExtendedPermissions { get; }

        /// <summary>
        /// Список секций, по которым игнорируется проверка расширенных прав доступа
        /// </summary>
        ICollection<string> IgnoreSections { get; }

        /// <summary>
        /// Доп. информация
        /// </summary>
        IDictionary<string, object> Info { get; }

        /// <summary>
        /// Билдер результата валидации
        /// </summary>
        IValidationResultBuilder ValidationResult { get; }

        /// <summary>
        /// Объект для доступа к базе данных
        /// </summary>
        IDbScope DbScope { get; }

        /// <summary>
        /// Сессия текущего сотрудника
        /// </summary>
        ISession Session { get; }
    }
}