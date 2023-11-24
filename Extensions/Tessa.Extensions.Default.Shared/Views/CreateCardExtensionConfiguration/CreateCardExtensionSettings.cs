// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCardExtensionSettings.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Views
{
    /// <summary>
    /// The create card extension settings.
    /// </summary>
    public sealed class CreateCardExtensionSettings :
        IStorageSerializable
    {
        #region Properties

        /// <summary>
        /// Режим создания карточки.
        /// </summary>
        public CardCreationKind CardCreationKind { get; set; }
        
        /// <summary>
        /// Режим открытия созданной карточки карточки. Игнорируется, если расширение используется для выбора ссылки по троеточию.
        /// </summary>
        public CardOpeningKind CardOpeningKind { get; set; }

        /// <summary>
        /// Алиас типа карточки, если режим создания карточки <see cref="CardCreationKind"/>
        /// равен <see cref="Views.CardCreationKind.ByTypeAlias"/>.
        /// </summary>
        public string TypeAlias { get; set; }

        /// <summary>
        /// Идентификатор типа документа (но не типа карточки), если режим создания карточки <see cref="CardCreationKind"/>
        /// равен <see cref="Views.CardCreationKind.ByDocTypeIdentifier"/>.
        /// </summary>
        public string DocTypeIdentifier { get; set; }

        /// <summary>
        /// Название параметра, по которому можно получить запись по первичному ключу.
        /// Необходимо для поведения "Создать новую карточку и выбрать" при выборе ссылки по троеточию.
        /// </summary>
        public string IDParam { get; set; }
        
        #endregion

        #region IStorageSerializable Members

        public IStorageSerializable Serialize(Dictionary<string, object> storage)
        {
            storage[nameof(this.CardCreationKind)] = (int)this.CardCreationKind;
            storage[nameof(this.CardOpeningKind)] = (int)this.CardOpeningKind;
            storage[nameof(this.TypeAlias)] = this.TypeAlias;
            storage[nameof(this.DocTypeIdentifier)] = this.DocTypeIdentifier;
            storage[nameof(this.IDParam)] = this.IDParam;
            return this;
        }

        public IStorageSerializable Deserialize(Dictionary<string, object> storage)
        {
            this.CardCreationKind = (CardCreationKind)storage.TryGet(nameof(this.CardCreationKind), (int)CardCreationKind.ByTypeFromSelection);
            this.CardOpeningKind = (CardOpeningKind)storage.TryGet(nameof(this.CardOpeningKind), (int)CardOpeningKind.ApplicationTab);
            this.TypeAlias = storage.TryGet<string>(nameof(this.TypeAlias));
            this.DocTypeIdentifier = storage.TryGet<string>(nameof(this.DocTypeIdentifier));
            this.IDParam = storage.TryGet<string>(nameof(this.IDParam));
            return this;
        }

        #endregion
    }
}