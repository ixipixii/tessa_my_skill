using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    public static class KrPermissionsHelper
    {
        #region Nested Types

        /// <summary>
        /// Список настроек доступа к полям. Берется из таблицы KrPermissionRuleAccessSettings в схеме
        /// </summary>
        public static class AccessSettings
        {
            public const int AllowEdit = 0;
            public const int DisallowEdit = 1;
            public const int DisallowRowAdding = 2;
            public const int DisallowRowDeleting = 3;
            public const int MaskData = 4;
        }

        /// <summary>
        /// Список типов проверки обязательности полей и секций
        /// </summary>
        public static class MandatoryValidationType
        {
            public const int Always = 0;
            public const int OnTaskCompletion = 1;
            public const int WhenOneFieldFilled = 2;
        }
        
        /// <summary>
        /// Список типов контролов для настроек видимости
        /// </summary>
        public static class ControlType
        {
            public const int Tab = 0;
            public const int Block = 1;
            public const int Control = 2;
        }

        public static class FileAccessSettings
        {
            public const int FileNotAvailable = 0;
            public const int ContentNotAvailable = 1;
            public const int OnlyLastVersion = 2;
            public const int OnlyLastAndOwnVersions = 3;

            public const string InfoKey = CardHelper.SystemKeyPrefix + nameof(FileAccessSettings);
        }

        #endregion

        #region Consts

        public const string CalculateSuperModeratorPermissions = "kr_calculate_supermoderator_permissions";

        public const string CalculateAddTopicPermissions = "kr_calculate_addtopic_permissions";

        public const string SuperModeratorPermissionsCalculated = "kr_supermoderator_permissions_calculated";

        public const string AddTopicPermissionsCalculated = "kr_addtopic_permissions_calculated";

        public const string SaveWithPermissionsCalcFlag = CardHelper.SystemKeyPrefix + "SaveWithPermissionsCalc";
       
        public const string CalculatePermissionsMark = "kr_calculate_permissions";

        public const string CalculateResolutionPermissionsMark = "kr_calculate_resolution_permissions";

        public const string PermissionsCalculatedMark = "kr_permissions_calculated";

        public const string UnavaliableTypesKey = ".unavailableTypes";

        public const string NewCardSourceKey = CardHelper.SystemKeyPrefix + "CardSource";

        public const string ServerTokenKey = CardHelper.SystemKeyPrefix + "KrServerToken";

        public const string SystemTable = "KrPermissionsSystem";

        public const string DropPermissionsCacheMark = CardHelper.SystemKeyPrefix + "DropPermissionsCache";

        public const string FailedMandatoryRulesKey = CardHelper.SystemKeyPrefix + "FailedMandatoryRules";

        #endregion

        #region Static Methods

        /// <summary>
        /// Возвращает список недоступных имен для создания эффективных (типы карточек, не использующие типы документов и типы документов) типов.
        /// </summary>
        /// <param name="cardRepository">Репозиторий карточек</param>
        /// <param name="krTypesCache">Кеш типов</param>
        /// <param name="includeHiddenTypes">Включать ли скрытые типы</param>
        /// <returns>Возвращает список недоступных имен для создания эффективных типов.</returns>
        public static async Task<ReadOnlyCollection<Guid>> GetUnavailableTypesAsync(
            ICardRepository cardRepository,
            IKrTypesCache krTypesCache, 
            bool includeHiddenTypes = false,
            CancellationToken cancellationToken = default)
        {
            var request = new CardRequest { RequestType = DefaultRequestTypes.GetUnavailableTypes };
            CardResponse response = await cardRepository.RequestAsync(request, cancellationToken);

            if (!response.Info.TryGetValue(UnavaliableTypesKey, out object unavailableTypesObj))
            {
                return EmptyHolder<Guid>.Collection;
            }

            var unavailableTypes = (IList<object>)unavailableTypesObj;
            var result = unavailableTypes.Cast<Guid>().ToList();

            if (!includeHiddenTypes)
            {
                foreach (IKrType krType in await krTypesCache.GetTypesAsync(cancellationToken))
                {
                    if (krType.HideCreationButton && !result.Contains(krType.ID))
                    {
                        result.Add(krType.ID);
                    }
                }
            }

            return result.AsReadOnly();
        }

        /// <summary>
        /// Формирует сообщение об ошибке недостаточности прав.
        /// </summary>
        /// <param name="stillRequired">Список прав, которых не хватает</param>
        /// <returns>Сообщение об ошибке</returns>
        public static string GetNotEnoughPermissionsErrorMessage(params KrPermissionFlagDescriptor[] stillRequired)
        {
            return LocalizationManager.GetString("KrMessages_NoPermissionsTo")
                + GetPermissionsSplittedByNewLineStartsWithNewLine(stillRequired);
        }

        /// <summary>
        /// Формирует сообщение о выданных правах.
        /// </summary>
        /// <param name="granted">Список выданных прав</param>
        /// <returns>Сообщение о выданных правах</returns>
        public static string GetGrantedPermissionsMessage(params KrPermissionFlagDescriptor[] granted)
        {
            return LocalizationManager.GetString("KrMessages_PermissionsGranted")
                + GetPermissionsSplittedByNewLineStartsWithNewLine(granted);
        }

        /// <summary>
        /// Возвращает локализованные разрешения разделенные переводом на новую строку. Начинает новой строкой.
        /// </summary>
        /// <param name="permissions">Разрешения, которые нужно локализовать и перечислить.</param>
        /// <param name="addLeftPadding"></param>
        /// <returns>Локализованные разрешения разделенные переводом на новую строку. Начинает новой строкой.</returns>
        public static string GetPermissionsSplittedByNewLineStartsWithNewLine(
            params KrPermissionFlagDescriptor[] permissions)
        {
            if (permissions.Length == 0)
            {
                return string.Empty;
            }
            else if (permissions.Length == 1)
            {
                return Environment.NewLine + LocalizationManager.Localize(permissions[0].Description);
            }

            StringBuilder result = StringBuilderHelper.Acquire();

            string split = Environment.NewLine;
            
            foreach(var permission in permissions.OrderBy(x => x.Order))
            {
                result.Append(split).Append(LocalizationManager.Localize(permission.Description));
            }

            return result.ToStringAndRelease();
        }

        #endregion
    }
}
