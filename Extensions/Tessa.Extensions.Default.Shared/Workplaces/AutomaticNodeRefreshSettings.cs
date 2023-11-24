// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutomaticNodeRefreshSettings.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Shared.Workplaces
{
    #region

    using System;
    using System.Collections.Generic;

    using Tessa.Platform.Storage;

    #endregion

    /// <summary>
    ///     Настройки автоматического обновления узлов рабочего места.
    /// </summary>
    public class AutomaticNodeRefreshSettings : IAutomaticNodeRefreshSettings, IStorageSerializable
    {
        /// <inheritdoc />
        public AutomaticNodeRefreshSettings()
        {
            this.RefreshInterval = 300;
            this.WithContentDataRefreshing = true;
        }

        /// <inheritdoc />
        public int RefreshInterval { get; set; }

        /// <inheritdoc />
        public bool WithContentDataRefreshing { get; set; }

        #region IStorageSerializable Members

        public IStorageSerializable Serialize(Dictionary<string, object> storage)
        {
            storage[nameof(this.RefreshInterval)] = this.RefreshInterval;
            storage[nameof(this.WithContentDataRefreshing)] = this.WithContentDataRefreshing;
            return this;
        }

        public IStorageSerializable Deserialize(Dictionary<string, object> storage)
        {
            this.RefreshInterval = storage.TryGet<int>(nameof(this.RefreshInterval));
            this.WithContentDataRefreshing = storage.TryGet<bool>(nameof(this.WithContentDataRefreshing));
            return this;
        }

        #endregion
    }
}