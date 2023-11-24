using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    public sealed class KrPermissionSectionSettingsStorage : StorageObject, IKrPermissionSectionSettings
    {
        #region Nested Types

        private enum AccessFlag
        {
            None = 0,
            IsAllowed = 1,
            IsDisallowed = 2,
            IsHidden = 4,
            DisallowRowAdding = 8,
            DisallowRowDeleting = 16,
            IsMandatory = 32,
            IsMasked = 64,
        }

        #endregion

        #region Fields

        private IReadOnlyCollection<Guid> allowedFields;
        private IReadOnlyCollection<Guid> disallowedFields;
        private IReadOnlyCollection<Guid> hiddenFields;
        private IReadOnlyCollection<Guid> mandatoryFields;
        private IReadOnlyCollection<Guid> maskedFields;

        #endregion

        #region Constrcutors

        public KrPermissionSectionSettingsStorage(Dictionary<string, object> storage)
            :base(storage)
        {
            this.Init(nameof(ID), GuidBoxes.Empty);
            this.Init(nameof(Flag), Int32Boxes.Zero);
        }

        public KrPermissionSectionSettingsStorage(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
        }

        #endregion

        #region IKrPermissionSectionSettings Implementation

        public Guid ID
        {
            get => this.Get<Guid>(nameof(ID));
            set => this.Set(nameof(ID), value);
        }

        public bool DisallowRowAdding
        {
            get => this.Has(AccessFlag.DisallowRowAdding);
            set => this.Set(AccessFlag.DisallowRowAdding, value);
        }

        public bool DisallowRowDeleting
        {
            get => this.Has(AccessFlag.DisallowRowDeleting);
            set => this.Set(AccessFlag.DisallowRowDeleting, value);
        }

        public bool IsAllowed
        {
            get => this.Has(AccessFlag.IsAllowed);
            set => this.Set(AccessFlag.IsAllowed, value);
        }

        public bool IsDisallowed
        {
            get => this.Has(AccessFlag.IsDisallowed);
            set => this.Set(AccessFlag.IsDisallowed, value);
        }

        public bool IsHidden
        {
            get => this.Has(AccessFlag.IsHidden);
            set => this.Set(AccessFlag.IsHidden, value);
        }

        public bool IsMandatory
        {
            get => this.Has(AccessFlag.IsMandatory);
            set => this.Set(AccessFlag.IsMandatory, value);
        }

        public bool IsMasked
        {
            get => this.Has(AccessFlag.IsMasked);
            set => this.Set(AccessFlag.IsMasked, value);
        }

        public IReadOnlyCollection<Guid> AllowedFields
        {
            get => this.allowedFields ??= this.TryGet<List<object>>(nameof(this.AllowedFields))?.Cast<Guid>().ToList() ?? (IReadOnlyCollection<Guid>)EmptyHolder<Guid>.Collection;
            set
            {
                this.allowedFields = value;
                this.Set(nameof(AllowedFields), new List<object>(value.Cast<object>()));
            }
        }

        public IReadOnlyCollection<Guid> DisallowedFields
        {
            get => this.disallowedFields ??= this.TryGet<List<object>>(nameof(this.DisallowedFields))?.Cast<Guid>().ToList() ?? (IReadOnlyCollection<Guid>)EmptyHolder<Guid>.Collection;
            set
            {
                this.disallowedFields = value;
                this.Set(nameof(DisallowedFields), new List<object>(value.Cast<object>()));
            }
        }

        public IReadOnlyCollection<Guid> HiddenFields
        {
            get => this.hiddenFields ??= this.TryGet<List<object>>(nameof(this.HiddenFields))?.Cast<Guid>().ToList() ?? (IReadOnlyCollection<Guid>)EmptyHolder<Guid>.Collection;
            set
            {
                this.hiddenFields = value;
                this.Set(nameof(HiddenFields), new List<object>(value.Cast<object>()));
            }
        }

        public IReadOnlyCollection<Guid> MandatoryFields
        {
            get => this.mandatoryFields ??= this.TryGet<List<object>>(nameof(this.MandatoryFields))?.Cast<Guid>().ToList() ?? (IReadOnlyCollection<Guid>)EmptyHolder<Guid>.Collection;
            set
            {
                this.mandatoryFields = value;
                this.Set(nameof(MandatoryFields), new List<object>(value.Cast<object>()));
            }
        }

        public IReadOnlyCollection<Guid> MaskedFields
        {
            get => this.maskedFields ??= this.TryGet<List<object>>(nameof(this.MaskedFields))?.Cast<Guid>().ToList() ?? (IReadOnlyCollection<Guid>)EmptyHolder<Guid>.Collection;
            set
            {
                this.maskedFields = value;
                this.Set(nameof(MaskedFields), new List<object>(value.Cast<object>()));
            }
        }

        #endregion

        #region Storage Properties

        private AccessFlag Flag
        {
            get => (AccessFlag)this.Get<int>(nameof(Flag));
            set => this.Set(nameof(Flag), (int)value);
        }

        #endregion

        #region Private Methods

        private bool Has(AccessFlag flag)
        {
            return (Flag & flag) == flag;
        }

        private void Set(AccessFlag flag, bool value)
        {
            Flag =
                value
                ? Flag | flag
                : (Flag & ~flag);
        }

        #endregion
    }
}
