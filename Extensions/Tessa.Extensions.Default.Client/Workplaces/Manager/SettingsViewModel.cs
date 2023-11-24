// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces.Manager
{
    #region

    using System;
    using System.Collections.Generic;

    using Tessa.Properties.Resharper;
    using Tessa.UI;
    using Tessa.Extensions.Default.Shared.Workplaces;

    #endregion

    /// <summary>
    ///     Модель-представление для редактирования настроек рабочего места руководителя
    /// </summary>
    public sealed class SettingsViewModel : ViewModel<EmptyModel>
    {
        /// <summary>
        ///     The active image column name.
        /// </summary>
        private string activeImageColumnName;

        /// <summary>
        /// The card id.
        /// </summary>
        private Guid cardId;

        /// <summary>
        ///     The count column name.
        /// </summary>
        private string countColumnName;

        /// <summary>
        /// The hover image column name.
        /// </summary>
        private string hoverImageColumnName;

        /// <summary>
        ///     The inactive image column name.
        /// </summary>
        private string inactiveImageColumnName;

        /// <summary>
        ///     The tile column name.
        /// </summary>
        private string tileColumnName;

        /// <inheritdoc />
        public SettingsViewModel([NotNull] IEnumerable<string> columnNames)
        {
            if (columnNames == null)
            {
                throw new ArgumentNullException("columnNames");
            }

            this.ColumnNames = columnNames;
        }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для выбранной плитки
        /// </summary>
        [CanBeNull]
        public string ActiveImageColumnName
        {
            get
            {
                return this.activeImageColumnName;
            }

            set
            {
                if (value == this.activeImageColumnName)
                {
                    return;
                }

                this.activeImageColumnName = value;
                this.OnPropertyChanged("ActiveImageColumnName");
            }
        }

        /// <summary>
        ///     Gets or sets Идентификатор карточки содержащей изображения
        /// </summary>
        public Guid CardId
        {
            get
            {
                return this.cardId;
            }

            set
            {
                if (value.Equals(this.cardId))
                {
                    return;
                }

                this.cardId = value;
                this.OnPropertyChanged("CardId");
            }
        }

        /// <summary>
        ///     Gets Список доступных для выборки столбцов
        /// </summary>
        [NotNull]
        public IEnumerable<string> ColumnNames { get; private set; }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего количество
        /// </summary>
        [CanBeNull]
        public string CountColumnName
        {
            get
            {
                return this.countColumnName;
            }

            set
            {
                if (value == this.countColumnName)
                {
                    return;
                }

                this.countColumnName = value;
                this.OnPropertyChanged("CountColumnName");
            }
        }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для плитки над которой находится
        ///     курсор
        /// </summary>
        public string HoverImageColumnName
        {
            get
            {
                return this.hoverImageColumnName;
            }

            set
            {
                if (value == this.hoverImageColumnName)
                {
                    return;
                }

                this.hoverImageColumnName = value;
                this.OnPropertyChanged("HoverImageColumnName");
            }
        }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего изображение для невыбранной плитки
        /// </summary>
        [CanBeNull]
        public string InactiveImageColumnName
        {
            get
            {
                return this.inactiveImageColumnName;
            }

            set
            {
                if (value == this.inactiveImageColumnName)
                {
                    return;
                }

                this.inactiveImageColumnName = value;
                this.OnPropertyChanged("InactiveImageColumnName");
            }
        }

        /// <summary>
        ///     Gets or sets Имя столбца содержащего заголовок плитки
        /// </summary>
        [CanBeNull]
        public string TileColumnName
        {
            get
            {
                return this.tileColumnName;
            }

            set
            {
                if (value == this.tileColumnName)
                {
                    return;
                }

                this.tileColumnName = value;
                this.OnPropertyChanged("TileColumnName");
            }
        }

        /// <summary>
        /// Создает модель-представление и инициализирует ее свойства из настроек
        ///     задаваемых в параметре <paramref name="settings"/>
        /// </summary>
        /// <param name="settings">
        /// Настройки рабочего места руководителя
        /// </param>
        /// <param name="columnNames">
        /// Список доступных для выборки имен столбцов
        /// </param>
        /// <returns>
        /// Модель-представление редактирования настроек рабочего места руководителя
        /// </returns>
        [NotNull]
        public static SettingsViewModel Create([NotNull] ManagerWorkplaceSettings settings, [NotNull] IEnumerable<string> columnNames)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (columnNames == null)
            {
                throw new ArgumentNullException("columnNames");
            }

            return new SettingsViewModel(columnNames)
                       {
                           ActiveImageColumnName = settings.ActiveImageColumnName,
                           InactiveImageColumnName = settings.InactiveImageColumnName,
                           CountColumnName = settings.CountColumnName,
                           TileColumnName = settings.TileColumnName,
                           CardId = settings.CardId,
                           HoverImageColumnName = settings.HoverImageColumnName
                       };
        }
    }
}