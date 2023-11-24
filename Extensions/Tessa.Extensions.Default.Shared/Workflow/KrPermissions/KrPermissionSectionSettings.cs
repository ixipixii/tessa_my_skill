using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    /// <summary>
    /// Набор настроек для секций в правилах доступа
    /// </summary>
    public sealed class KrPermissionSectionSettings : IKrPermissionSectionSettings
    {
        #region IKrPermissionSectionSettings Properties

        public Guid ID { get; set; }

        public bool DisallowRowAdding { get; set; }

        public bool DisallowRowDeleting { get; set; }

        public bool IsAllowed { get; set; }

        public bool IsDisallowed { get; set; }

        public bool IsHidden { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsMasked { get; set; }

        IReadOnlyCollection<Guid> IKrPermissionSectionSettings.AllowedFields { get => AllowedFields; set => this.AllowedFields = new HashSet<Guid>(value); }

        IReadOnlyCollection<Guid> IKrPermissionSectionSettings.DisallowedFields { get => DisallowedFields; set => this.DisallowedFields = new HashSet<Guid>(value); }

        IReadOnlyCollection<Guid> IKrPermissionSectionSettings.HiddenFields { get => HiddenFields; set => this.HiddenFields = new HashSet<Guid>(value); }

        IReadOnlyCollection<Guid> IKrPermissionSectionSettings.MandatoryFields { get => MandatoryFields; set => this.MandatoryFields = new HashSet<Guid>(value); }

        IReadOnlyCollection<Guid> IKrPermissionSectionSettings.MaskedFields { get => MaskedFields; set => this.MaskedFields = new HashSet<Guid>(value); }

        #endregion

        #region Properties

        public string Mask { get; set; }

        public HashSet<Guid> AllowedFields { get; set; } = new HashSet<Guid>();

        public HashSet<Guid> DisallowedFields { get; set; } = new HashSet<Guid>();

        public HashSet<Guid> HiddenFields { get; set; } = new HashSet<Guid>();

        public HashSet<Guid> MandatoryFields { get; set; } = new HashSet<Guid>();

        public HashSet<Guid> MaskedFields { get; set; } = new HashSet<Guid>();

        public Dictionary<Guid, string> MaskedFieldsData { get; set; } = new Dictionary<Guid, string>();

        #endregion

        #region Public Methods

        public KrPermissionSectionSettings Clone()
        {
            return CreateFrom(this);
        }

        public void MergeWith(IKrPermissionSectionSettings sectionSettings, bool rewriteFieldAllowance = false)
        {
            if (this.ID != sectionSettings.ID)
            {
                return;
            }

            if (rewriteFieldAllowance)
            {
                this.DisallowedFields.RemoveRange(sectionSettings.AllowedFields);
                this.AllowedFields.RemoveRange(sectionSettings.DisallowedFields);
            }
            // Если хотя бы в одном задано, значит так оно и есть
            this.DisallowRowAdding |= sectionSettings.DisallowRowAdding;
            this.DisallowRowDeleting |= sectionSettings.DisallowRowDeleting;
            this.IsAllowed |= sectionSettings.IsAllowed;
            this.IsDisallowed |= sectionSettings.IsDisallowed;

            this.IsHidden |= sectionSettings.IsHidden;
            this.IsMandatory |= sectionSettings.IsMandatory;

            // Списки полей просто мержим
            this.AllowedFields.AddRange(sectionSettings.AllowedFields);
            this.DisallowedFields.AddRange(sectionSettings.DisallowedFields);
            this.HiddenFields.AddRange(sectionSettings.HiddenFields);
            this.MandatoryFields.AddRange(sectionSettings.MandatoryFields);
            this.MaskedFields.AddRange(sectionSettings.MaskedFields);

            if (sectionSettings is KrPermissionSectionSettings settignsWithMaskedFields)
            {
                // Замаскированные объединяем, а данные по полям заменяем
                foreach(var pair in settignsWithMaskedFields.MaskedFieldsData)
                {
                    this.MaskedFieldsData[pair.Key] = pair.Value;
                }
            }
        }

        public void Clean()
        {
            if (this.IsHidden)
            {
                this.HiddenFields.Clear();
            }

            if (this.IsMasked)
            {
                this.DisallowRowAdding = false;
                this.DisallowRowDeleting = false;
                this.IsAllowed = false;
                this.IsDisallowed = true;
                this.AllowedFields.Clear();
                return;
            }

            // Если стоит IsDisallowed, то остальные флаги сбрасываем
            if (this.IsDisallowed)
            {
                this.DisallowRowAdding = false;
                this.DisallowRowDeleting = false;
                this.IsAllowed = false;
            }

            // Удаляем доступные поля, которые есть в списках недоступных
            this.DisallowedFields.AddRange(this.MaskedFields);
            this.AllowedFields.RemoveWhere(x => this.DisallowedFields.Contains(x));
        }

        public bool CheckAndClean(CardType cardType)
        {
            var schemeItem = cardType.SchemeItems.FirstOrDefault(x => x.SectionID == ID);

            if (schemeItem == null)
            {
                return false;
            }

            Clean();

            // Удаляем поля, которых нет в картчоке данного типа
            this.AllowedFields.IntersectWith(schemeItem.ColumnIDList);
            this.DisallowedFields.IntersectWith(schemeItem.ColumnIDList);
            this.HiddenFields.IntersectWith(schemeItem.ColumnIDList);
            this.MandatoryFields.IntersectWith(schemeItem.ColumnIDList);
            this.MaskedFields.IntersectWith(schemeItem.ColumnIDList);

            return true;
        }

        public static KrPermissionSectionSettings ConvertFrom(IKrPermissionSectionSettings setting)
        {
            if (setting is KrPermissionSectionSettings settingsTypified)
            {
                return settingsTypified;
            }
            else
            {
                return CreateFrom(setting);
            }
        }

        #endregion

        #region Private Methods

        private static KrPermissionSectionSettings CreateFrom(IKrPermissionSectionSettings setting)
        {
            var result = 
                new KrPermissionSectionSettings
                {
                    ID = setting.ID,
                    DisallowRowAdding = setting.DisallowRowAdding,
                    DisallowRowDeleting = setting.DisallowRowDeleting,
                    IsAllowed = setting.IsAllowed,
                    IsDisallowed = setting.IsDisallowed,
                    IsHidden = setting.IsHidden,
                    IsMandatory = setting.IsMandatory,
                    IsMasked = setting.IsMasked,
                    AllowedFields = new HashSet<Guid>(setting.AllowedFields),
                    DisallowedFields = new HashSet<Guid>(setting.DisallowedFields),
                    HiddenFields = new HashSet<Guid>(setting.HiddenFields),
                    MandatoryFields = new HashSet<Guid>(setting.MandatoryFields),
                    MaskedFields = new HashSet<Guid>(setting.MaskedFields),
                };

            if (setting is KrPermissionSectionSettings settingWithMask)
            {
                result.Mask = settingWithMask.Mask;
                result.MaskedFieldsData = new Dictionary<Guid, string>(settingWithMask.MaskedFieldsData);
            }

            return result;
        }

        #endregion
    }
}
