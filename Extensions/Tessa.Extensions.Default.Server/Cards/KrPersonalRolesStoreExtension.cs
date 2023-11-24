using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Platform.Shared.Initialization;
using Tessa.Roles;
using Tessa.Extensions.Platform.Server.Roles;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Расширение для предоставления доступа сотрудникам редактировать свои настройки
    /// и для установки прав на редактирование некоторых полей сотрудника, заполняемых в расширениях <see cref="PersonalRoleStoreExtension"/>
    /// и <see cref="FixPersonalRolesStoreExtension" />
    /// </summary>
    public sealed class KrPersonalRolesStoreExtension : CardStoreExtension
    {
        #region Fields

        private readonly IKrTypesCache typesCache;

        #endregion

        #region Constructors

        public KrPersonalRolesStoreExtension(
            IKrTypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        #endregion

        #region Base Overrides

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, typesCache))
            {
                return;
            }

            if (context.Request.Card.ID == context.Session.User.ID)
            {
                // Всегда разрешаем редактировать свои настройки
                await context.SetCardAccessAsync(
                    InitializationExtensionHelper.PersonalRoleVirtualSection,
                    InitializationExtensionHelper.PersonalRoleSettingsField,
                    InitializationExtensionHelper.PersonalRoleNotificationSettingsField);
            }

            // Для новой карточки разрешаем изменение информации, заполненное системой
            if (context.Request.Card.StoreMode == Tessa.Cards.CardStoreMode.Insert)
            {
                await context.SetCardAccessAsync(
                    InitializationExtensionHelper.PersonalRoleVirtualSection,
                    InitializationExtensionHelper.PersonalRoleSettingsField,
                    InitializationExtensionHelper.PersonalRoleNotificationSettingsField);

                await context.SetCardAccessAsync(
                    RoleStrings.Roles,
                    "Name");

                await context.SetCardAccessAsync(
                    RoleStrings.PersonalRoles,
                    "Name",
                    "FullName",
                    "PasswordChanged");

                await context.SetCardAccessAsync(
                    RoleStrings.RoleUsers);
            }
            context.Request.SetIgnorePermissionsWarning(); 
        }

        #endregion
    }
}
