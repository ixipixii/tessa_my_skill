using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Platform.Collections;
using Tessa.Platform.IO;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <inheritdoc />
    public sealed class KrPermissionsManagerResult : IKrPermissionsManagerResult
    {
        #region Fields

        private readonly KrPermissionsDescriptor descriptor;
        private readonly bool withExtendedPermissions;

        #endregion

        #region Constructors

        public KrPermissionsManagerResult(
            KrPermissionsDescriptor descriptor,
            bool withExtendedPermissions,
            long version)
        {
            this.descriptor = descriptor;
            this.withExtendedPermissions = withExtendedPermissions;
            this.Version = version;
        }

        private KrPermissionsManagerResult()
        {
            this.descriptor = new KrPermissionsDescriptor();
        }

        #endregion

        #region IKrPermissionsManagerResult Implementation

        /// <inheritdoc />
        public long Version { get; }

        /// <inheritdoc />
        public bool WithEdit => withExtendedPermissions
            && (descriptor.Permissions.Contains(KrPermissionFlagDescriptors.EditCard)
                || descriptor.StillRequired.Contains(KrPermissionFlagDescriptors.EditCard));

        /// <inheritdoc />
        public ICollection<KrPermissionFlagDescriptor> Permissions => descriptor.Permissions;

        /// <inheritdoc />
        public HashSet<Guid, KrPermissionSectionSettings> ExtendedCardSettings => descriptor.ExtendedCardSettings;

        /// <inheritdoc />
        public Dictionary<Guid, HashSet<Guid, KrPermissionSectionSettings>> ExtendedTasksSettings 
            => descriptor.ExtendedTasksSettings;

        /// <inheritdoc />
        public ICollection<KrPermissionFileRule> FileRules
            => descriptor.FileRules;

        /// <inheritdoc />
        public bool Has(KrPermissionFlagDescriptor krPermission)
        {
            return descriptor.Has(krPermission);
        }

        /// <inheritdoc />
        public IKrPermissionExtendedCardSettings CreateExtendedCardSettings(Guid userID, Card card)
        {
            var result = new KrPermissionExtendedCardSettingsStorage();

            if (descriptor.ExtendedCardSettings.Count > 0)
            {
                foreach (var settings in descriptor.ExtendedCardSettings)
                {
                    var newSettings = result.SectionSettings.Add();

                    newSettings.ID = settings.ID;
                    newSettings.IsAllowed = settings.IsAllowed;
                    newSettings.IsDisallowed = settings.IsDisallowed;
                    newSettings.IsHidden = settings.IsHidden;
                    newSettings.IsMandatory = settings.IsMandatory;
                    newSettings.DisallowRowAdding = settings.DisallowRowAdding;
                    newSettings.DisallowRowDeleting = settings.DisallowRowDeleting;
                    if (settings.AllowedFields.Count > 0)
                    {
                        newSettings.AllowedFields = settings.AllowedFields;
                    }
                    if (settings.DisallowedFields.Count > 0)
                    {
                        newSettings.DisallowedFields = settings.DisallowedFields;
                    }
                    if (settings.HiddenFields.Count > 0)
                    {
                        newSettings.HiddenFields = settings.HiddenFields;
                    }
                    if (settings.MandatoryFields.Count > 0)
                    {
                        newSettings.MandatoryFields = settings.MandatoryFields;
                    }
                    if (settings.MaskedFields.Count > 0)
                    {
                        newSettings.MaskedFields = settings.MaskedFields;
                    }
                }
            }

            foreach(var settingsByTaskType in descriptor.ExtendedTasksSettings)
            {
                if (settingsByTaskType.Value.Count > 0)
                {
                    result.TaskSettingsTypes.Add(settingsByTaskType.Key);
                    var taskSettings = result.TaskSettings.Add();
                    foreach (var settings in settingsByTaskType.Value)
                    {
                        var newSettings = taskSettings.Add();

                        newSettings.ID = settings.ID;
                        newSettings.IsAllowed = settings.IsAllowed;
                        newSettings.IsDisallowed = settings.IsDisallowed;
                        newSettings.IsHidden = settings.IsHidden;
                        newSettings.DisallowRowAdding = settings.DisallowRowAdding;
                        newSettings.DisallowRowDeleting = settings.DisallowRowDeleting;
                        if (settings.AllowedFields.Count > 0)
                        {
                            newSettings.AllowedFields = settings.AllowedFields;
                        }
                        if (settings.DisallowedFields.Count > 0)
                        {
                            newSettings.DisallowedFields = settings.DisallowedFields;
                        }
                        if (settings.HiddenFields.Count > 0)
                        {
                            newSettings.HiddenFields = settings.HiddenFields;
                        }
                    }
                }
            }

            foreach (var setting in descriptor.VisibilitySettings)
            {
                result.VisibilitySettings.Add(
                    setting.ToStorage());
            }

            if (descriptor.FileRules.Count > 0)
            {
                for (int i = card.Files.Count - 1; i >= 0; i--)
                {
                    var file = card.Files[i];

                    // Данные правила не распространяются на виртуальные файлы
                    if (file.IsVirtual)
                    {
                        continue;
                    }

                    bool isOwnFile = userID == file.Card.CreatedByID;
                    string fileExtension = FileHelper.GetExtension(file.Name).TrimStart('.');
                    int currentAccessSetting = int.MaxValue;
                    foreach (var fileRule in descriptor.FileRules)
                    {
                        if (fileRule.AccessSetting < currentAccessSetting
                            && (!isOwnFile
                                || fileRule.CheckOwnFiles)
                            && (fileRule.Extensions.Count == 0
                                || fileRule.Extensions.Contains(fileExtension))
                            && (fileRule.Categories.Count == 0
                                || (file.CategoryID is Guid categoryID
                                    && fileRule.Categories.Any(x => x == categoryID))))
                        {
                            currentAccessSetting = fileRule.AccessSetting;
                            if (currentAccessSetting == KrPermissionsHelper.FileAccessSettings.FileNotAvailable)
                            {
                                // Если нашли правило с полным запретом, то нет смысла проверять дальше
                                break;
                            }
                        }
                    }

                    if (currentAccessSetting != int.MaxValue)
                    {
                        var fileSetting = result.FileSettings.Add();

                        fileSetting.FileID = file.RowID;
                        fileSetting.AccessSetting = currentAccessSetting;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Static

        public static IKrPermissionsManagerResult Empty = new KrPermissionsManagerResult();

        #endregion
    }
}
