#region Usings

using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

#endregion

namespace Tessa.Extensions.Default.Shared.Workplaces
{
    [Serializable]
    public sealed class TreeItemFilteringSettings : ITreeItemFilteringSettings, IStorageSerializable
    {
        #region Fields

        private List<string> parameters;
        private List<string> refSections;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public TreeItemFilteringSettings()
        {
            this.RefSections = new List<string>();
            this.Parameters = new List<string>();
        }

        #endregion

        #region Public properties

        /// <inheritdoc />
        public List<string> Parameters
        {
            get => this.parameters;
            set => this.parameters = value ?? new List<string>();
        }

        /// <inheritdoc />
        public List<string> RefSections
        {
            get => this.refSections;
            set => this.refSections = value ?? new List<string>();
        }

        #endregion

        #region IStorageSerializable Members

        public IStorageSerializable Serialize(Dictionary<string, object> storage)
        {
            storage[nameof(this.Parameters)] = this.Parameters;
            storage[nameof(this.RefSections)] = this.RefSections;
            return this;
        }

        public IStorageSerializable Deserialize(Dictionary<string, object> storage)
        {
            this.Parameters = storage.TryGet<List<string>>(nameof(this.Parameters));
            this.RefSections = storage.TryGet<List<string>>(nameof(this.RefSections));
            return this;
        }

        #endregion
    }
}