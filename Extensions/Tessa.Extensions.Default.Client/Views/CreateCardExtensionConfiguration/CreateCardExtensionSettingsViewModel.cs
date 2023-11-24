// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCardExtensionSettingsViewModel.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using System;
    using System.Collections.ObjectModel;

    using Tessa.Extensions.Default.Shared.Views;
    using Tessa.Properties.Resharper;
    using Tessa.UI;

    /// <summary>
    ///     The create card extension settings view model.
    /// </summary>
    public sealed class CreateCardExtensionSettingsViewModel : ViewModel<EmptyModel>
    {
        /// <summary>
        ///     The card creation mode.
        /// </summary>
        private CardCreationKind cardCreationMode;

        /// <summary>
        ///     The card opening mode.
        /// </summary>
        private CardOpeningKind cardOpeningMode;

        /// <summary>
        ///     The type alias.
        /// </summary>
        [CanBeNull]
        private string typeAlias;

        /// <summary>
        ///     The type identifier.
        /// </summary>
        [CanBeNull]
        private string docTypeIdentifier;

        private string idParam;
        
        /// <inheritdoc />
        public CreateCardExtensionSettingsViewModel()
        {
            foreach (var mode in Enum.GetValues(typeof(CardCreationKind)))
            {
                this.CardCreationModes.Add((CardCreationKind)mode);
            }

            foreach (var mode in Enum.GetValues(typeof(CardOpeningKind)))
            {
                this.CardOpeningModes.Add((CardOpeningKind)mode);
            }

            this.cardCreationMode = CardCreationKind.ByTypeFromSelection;
            this.cardOpeningMode = CardOpeningKind.ApplicationTab;
        }

        /// <summary>
        /// Выбранный режим создания карточки.
        /// </summary>
        public CardCreationKind CardCreationMode
        {
            get => this.cardCreationMode;
            set
            {
                if (this.cardCreationMode != value)
                {
                    this.cardCreationMode = value;
                    this.OnPropertyChanged(nameof(this.CardCreationMode));

                    this.TypeAlias = null;
                    this.DocTypeIdentifier = null;
                }
            }
        }

        /// <summary>
        ///     Gets Список доступных режимов создания карточки
        /// </summary>
        [NotNull]
        public ObservableCollection<CardCreationKind> CardCreationModes { get; } =
            new ObservableCollection<CardCreationKind>();

        /// <summary>
        /// Выбранный режим открытия карточки.
        /// </summary>
        public CardOpeningKind CardOpeningMode
        {
            get => this.cardOpeningMode;
            set
            {
                if (this.cardOpeningMode != value)
                {
                    this.cardOpeningMode = value;
                    this.OnPropertyChanged(nameof(this.CardOpeningMode));
                }
            }
        }

        /// <summary>
        ///     Gets Список доступных режимов открытия карточки
        /// </summary>
        [NotNull]
        public ObservableCollection<CardOpeningKind> CardOpeningModes { get; } =
            new ObservableCollection<CardOpeningKind>();

        /// <summary>
        ///     Gets or sets Псевдоним типа карточки
        /// </summary>
        [CanBeNull]
        public string TypeAlias
        {
            get => this.typeAlias;
            set
            {
                if (value == this.typeAlias)
                {
                    return;
                }

                this.typeAlias = value;
                this.OnPropertyChanged(nameof(this.TypeAlias));
            }
        }

        /// <summary>
        ///     Gets or sets Идентификатор типа карточки
        /// </summary>
        [CanBeNull]
        public string DocTypeIdentifier
        {
            get => this.docTypeIdentifier;
            set
            {
                if (value == this.docTypeIdentifier)
                {
                    return;
                }

                this.docTypeIdentifier = value;
                this.OnPropertyChanged(nameof(this.DocTypeIdentifier));
            }
        }
        
        /// <summary>
        /// Название параметра, по которому можно получить запись по первичному ключу.
        /// Необходимо для поведения "Создать новую карточку и выбрать".
        /// </summary>
        [CanBeNull]
        public string IDParam
        {
            get => this.idParam;
            set
            {
                if (value == this.idParam)
                {
                    return;
                }

                this.idParam = value;
                this.OnPropertyChanged(nameof(this.IDParam));
            }
        }

        public void ApplyTo(CreateCardExtensionSettings settings)
        {
            settings.CardCreationKind = this.cardCreationMode;
            settings.CardOpeningKind = this.cardOpeningMode;
            settings.TypeAlias = this.typeAlias;
            settings.DocTypeIdentifier = this.docTypeIdentifier;
            settings.IDParam = this.idParam;
        }

        /// <summary>
        /// Создает модель-представление редактирования настроек по модели настроек
        /// </summary>
        /// <param name="settings">
        /// Настройки
        /// </param>
        /// <returns>
        /// Модель представления редактирования настроек
        /// </returns>
        [NotNull]
        public static CreateCardExtensionSettingsViewModel Create([NotNull] CreateCardExtensionSettings settings)
        {
            return new CreateCardExtensionSettingsViewModel
                   {
                       cardCreationMode = settings.CardCreationKind,
                       cardOpeningMode = settings.CardOpeningKind,
                       typeAlias = settings.TypeAlias,
                       docTypeIdentifier = settings.DocTypeIdentifier,
                       idParam = settings.IDParam,
                   };
        }
    }
}