// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Syntellect">
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
    ///     Настройки для рабочего места руководителя
    /// </summary>
    [Serializable]
    public class ManagerWorkplaceSettings :
        IStorageSerializable
    {
        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для выбранной плитки
        /// </summary>
        public string ActiveImageColumnName { get; set; }

        /// <summary>
        ///     Gets or sets Идентификатор карточки
        /// </summary>
        public Guid CardId { get; set; }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего количество
        /// </summary>
        public string CountColumnName { get; set; }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для плитки над которой находится курсор
        /// </summary>
        public string HoverImageColumnName { get; set; }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для невыбранной плитки
        /// </summary>
        public string InactiveImageColumnName { get; set; }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего заголовок плитки
        /// </summary>
        public string TileColumnName { get; set; }

        #region IStorageSerializable Members

        public IStorageSerializable Serialize(Dictionary<string, object> storage)
        {
            storage[nameof(this.ActiveImageColumnName)] = this.ActiveImageColumnName;
            storage[nameof(this.CardId)] = this.CardId;
            storage[nameof(this.CountColumnName)] = this.CountColumnName;
            storage[nameof(this.HoverImageColumnName)] = this.HoverImageColumnName;
            storage[nameof(this.InactiveImageColumnName)] = this.InactiveImageColumnName;
            storage[nameof(this.TileColumnName)] = this.TileColumnName;
            return this;
        }

        public IStorageSerializable Deserialize(Dictionary<string, object> storage)
        {
            this.ActiveImageColumnName = storage.TryGet<string>(nameof(this.ActiveImageColumnName));
            this.CardId = storage.TryGet<Guid>(nameof(this.CardId));
            this.CountColumnName = storage.TryGet<string>(nameof(this.CountColumnName));
            this.HoverImageColumnName = storage.TryGet<string>(nameof(this.HoverImageColumnName));
            this.InactiveImageColumnName = storage.TryGet<string>(nameof(this.InactiveImageColumnName));
            this.TileColumnName = storage.TryGet<string>(nameof(this.TileColumnName));
            return this;
        }

        #endregion
    }
}
