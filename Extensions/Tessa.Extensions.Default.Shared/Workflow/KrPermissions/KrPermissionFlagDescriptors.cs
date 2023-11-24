using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    public static class KrPermissionFlagDescriptors
    {
        /// <summary>
        /// Это поле должно быть выше вызовов конструкторов KrPermissionFlagDescriptor, т.к. порядок инициализации в статическом конструкторе
        /// </summary>
        private static readonly ConcurrentDictionary<KrPermissionFlagDescriptor, object> allDescriptors
            = new ConcurrentDictionary<KrPermissionFlagDescriptor, object>();

        /// <summary>
        /// Создание карточки.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor CreateCard = new KrPermissionFlagDescriptor(
            new Guid("{E63C8EF1-0B65-4348-B4A7-03AC58D52280}"),
            "CreateCard",
            0,
            "$KrPermissions_CreateCard",
            "$CardTypes_Controls_CreateCard",
            null,
            "CanCreateCard");

        /// <summary>
        /// Создание шаблона и копирование.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor CreateTemplateAndCopy = new KrPermissionFlagDescriptor(
            new Guid("{BE8935B1-9320-4A1E-8539-0D1E62068874}"),
            "CreateTemplateAndCopy",
            3,
            "$KrPermissions_CreateTemplateAndCopy",
            "$CardTypes_Controls_CreateTemplateAndCopy",
            null,
            "CanCreateTemplateAndCopy");

        /// <summary>
        /// Чтение карточки.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor ReadCard = new KrPermissionFlagDescriptor(
            new Guid("{745075B2-592D-4A1A-80B3-40BDD59F9359}"),
            "ReadCard",
            5,
            "$KrPermissions_ReadCard",
            "$CardTypes_Controls_CardRead",
            null,
            "CanReadCard");

        /// <summary>
        /// Редактирование данных карточки.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor EditCard = new KrPermissionFlagDescriptor(
            new Guid("{28AB461E-E6AA-44D6-948A-2892F6E79B48}"),
            "EditCard",
            10,
            "$KrPermissions_EditCard",
            "$CardTypes_Controls_CardEdit",
            null,
            "CanEditCard");

        /// <summary>
        /// Редактирование маршрута согласования.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor EditRoute = new KrPermissionFlagDescriptor(
            new Guid("{312E2DA3-8695-41E0-9643-9A763BC6FF46}"),
            "EditRoute",
            15,
            "$KrPermissions_EditRoute",
            "$CardTypes_Controls_EditApprovalRoute",
            null,
            "CanEditRoute");

        /// <summary>
        /// Возможность редактирования/выделения/освобождения номера
        /// </summary>
        public static readonly KrPermissionFlagDescriptor EditNumber = new KrPermissionFlagDescriptor(
            new Guid("{57468338-BC7C-422B-B649-1F1B356036E0}"),
            "EditNumber",
            20,
            "$KrPermissions_EditNumber",
            "$CardTypes_Controls_NumberManualEditing",
            null,
            "CanEditNumber");

        /// <summary>
        /// Возможность подписывать файлы
        /// </summary>
        public static readonly KrPermissionFlagDescriptor SignFiles = new KrPermissionFlagDescriptor(
            new Guid("{DCE82799-58F5-449F-A286-BA02604E292E}"),
            "SignFiles",
            25,
            "$KrPermissions_SignFiles",
            "$CardTypes_Controls_SignFiles",
            null,
            "CanSignFiles");

        /// <summary>
        /// Добавление файлов в карточку.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor AddFiles = new KrPermissionFlagDescriptor(
            new Guid("{196150EC-9ADF-48E3-A68C-EAFEAB258B44}"),
            "AddFiles",
            30,
            "$KrPermissions_AddFiles",
            "$CardTypes_Controls_AddFiles",
            null,
            "CanAddFiles");

        /// <summary>
        /// Можно редактировать свои файлы
        /// </summary>
        public static readonly KrPermissionFlagDescriptor EditOwnFiles = new KrPermissionFlagDescriptor(
            new Guid("{56AE3858-D7E6-44BA-B19B-FE2CC52A988C}"),
            "EditOwnFiles",
            35,
            "$KrPermissions_EditOwnFiles",
            "$CardTypes_Controls_EditOwnFiles",
            null,
            "CanEditOwnFiles");

        /// <summary>
        /// Редактирование файлов карточки.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor EditFiles = new KrPermissionFlagDescriptor(
            new Guid("{D83D0D34-7D67-4047-B2EB-BDECDD4EDAAF}"),
            "EditFiles",
            40,
            "$KrPermissions_EditFiles",
            "$CardTypes_Controls_EditAllFiles",
            "$CardTypes_Controls_EditAllFiles_Tooltip",
            "CanEditFiles",
            EditOwnFiles);

        /// <summary>
        /// Возможность удалять собственные файлы
        /// </summary>
        public static readonly KrPermissionFlagDescriptor DeleteOwnFiles = new KrPermissionFlagDescriptor(
            new Guid("{560F0CDE-8854-4985-B71A-9039F1BAEB49}"),
            "DeleteOwnFiles",
            45,
            "$KrPermissions_DeleteOwnFiles",
            "$CardTypes_Controls_DeleteOwnFiles",
            null,
            "CanDeleteOwnFiles");

        /// <summary>
        /// Возможность удалять файлы
        /// </summary>
        public static readonly KrPermissionFlagDescriptor DeleteFiles = new KrPermissionFlagDescriptor(
            new Guid("{431CEF5F-704E-4A5A-84D5-2AD2A95E5B64}"),
            "DeleteFiles",
            50,
            "$KrPermissions_DeleteFiles",
            "$CardTypes_Controls_DeleteFiles",
            null,
            "CanDeleteFiles",
            DeleteOwnFiles);

        /// <summary>
        /// Удаление карточки.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor DeleteCard = new KrPermissionFlagDescriptor(
            new Guid("{49AC8B82-0CE7-4745-AB2F-7464925A47FC}"),
            "DeleteCard",
            55,
            "$KrPermissions_DeleteCard",
            "$CardTypes_Controls_DeleteCard",
            null,
            "CanDeleteCard");

        /// <summary>
        /// Возможность создания резолюций
        /// </summary>
        public static readonly KrPermissionFlagDescriptor CreateResolutions = new KrPermissionFlagDescriptor(
            new Guid("{7DB8C661-039D-47D5-9773-0A92A03047DE}"),
            "CreateResolutions",
            60,
            "$KrPermissions_CreateResolutions",
            "$CardTypes_Controls_CreatingResolutions",
            null,
            "CanCreateResolutions");

        /// <summary>
        /// Возможность создания резолюций
        /// </summary>
        public static readonly KrPermissionFlagDescriptor AddTopics = new KrPermissionFlagDescriptor(
            new Guid("{C0998270-51C1-4014-ABF3-9B16BB8BA4E0}"),
            "AddTopics",
            65,
            "$KrPermissions_AddTopics",
            "$CardTypes_Controls_AddTopics",
            null,
            "CanAddTopics");

        /// <summary>
        /// Возможность создания резолюций
        /// </summary>
        public static readonly KrPermissionFlagDescriptor SuperModeratorMode = new KrPermissionFlagDescriptor(
            new Guid("{C7A4F3B9-D4DA-4980-B81E-B8A26E0AE6BF}"),
            "SuperModeratorMode",
            70,
            "$KrPermissions_SuperModeratorMode",
            "$CardTypes_Controls_SuperModeratorMode",
            null,
            "CanSuperModeratorMode",
            AddTopics);

        /// <summary>
        /// Возможность подписываться на уведомления карточки
        /// </summary>
        public static readonly KrPermissionFlagDescriptor SubscribeForNotifications = new KrPermissionFlagDescriptor(
            new Guid("{B0334858-FB8B-4EB4-81D9-55FD507141FE}"),
            "SubscribeForNotifications",
            75,
            "$KrPermissions_SubscribeForNotifications",
            "$CardTypes_Controls_SubscribeForNotifications",
            null,
            "CanSubscribeForNotifications");

        /// <summary>
        /// Возможность полного пересчёта маршрута.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor CanFullRecalcRoute = new KrPermissionFlagDescriptor(
            new Guid(0x28c691f5, 0x929d, 0x4cb9, 0xae, 0x4e, 0x61, 0x1d, 0xd9, 0xe2, 0xb0, 0xa1),
            "CanFullRecalcRoute",
            17,
            "$KrPermissions_CanFullRecalcRoute",
            "$CardTypes_Controls_CanFullRecalcRoute",
            null,
            "CanFullRecalcRoute");

        /// <summary>
        /// Возможность пропускать этапы маршрута.
        /// </summary>
        public static readonly KrPermissionFlagDescriptor CanSkipStages = new KrPermissionFlagDescriptor(
            new Guid(0x4d65e9e2, 0x9e73, 0x484e, 0xae, 0x3e, 0x4a, 0x76, 0x7a, 0xb5, 0x29, 0x4e),
            "CanSkipStages",
            18,
            "$KrPermissions_CanSkipStages",
            "$CardTypes_Controls_CanSkipStages",
            null,
            "CanSkipStages");

        /// <summary>
        /// Полные права на редактирование карточки (читать, редактировать карточку,
        /// добавлять и редактировать файлы, редактировать маршрут).
        /// Даются, напр, при наличии задания редактирования .
        /// </summary>
        public static readonly KrPermissionFlagDescriptor FullCardPermissionsGroup = new KrPermissionFlagDescriptor(
            new Guid("{6BC49BF3-4ACB-4072-822D-831EDC289C0E}"),
            "AllCardEditingAndAllFiles",
            ReadCard,
            EditCard,
            AddFiles,
            EditFiles,
            EditOwnFiles,
            DeleteFiles,
            DeleteOwnFiles,
            EditRoute,
            EditNumber,
            SignFiles,
            AddTopics,
            SubscribeForNotifications,
            CanFullRecalcRoute,
            CanSkipStages);

        /// <summary>
        /// Полный перечень всех прав доступа
        /// </summary>
        public static KrPermissionFlagDescriptor Full => GetFullDescriptor();


        internal static void AddDescriptor(KrPermissionFlagDescriptor flag)
        {
            allDescriptors.TryAdd(flag, null);
            full = null;
        }

        private static volatile KrPermissionFlagDescriptor full;

        public static KrPermissionFlagDescriptor GetFullDescriptor()
        {
            return full ??= new KrPermissionFlagDescriptor(
                new Guid("{7E1BC3B0-BE9D-41B2-961C-3463ABF09B89}"),
                "Full",
                allDescriptors.Keys.ToArray());
        }
    }
}
