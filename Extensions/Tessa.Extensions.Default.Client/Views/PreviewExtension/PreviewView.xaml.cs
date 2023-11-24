// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewView.xaml.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Tessa.PreviewHandlers;
    using Tessa.Properties.Resharper;
    using Tessa.UI.Files;
    using Tessa.UI.Views;
    using Tessa.UI.Views.Content;

    /// <summary>
    ///     Отображает предварительный просмотр файла.
    /// </summary>
    public partial class PreviewView : UserControl,
                                       IContentItem,
                                       INotifyPropertyChanged,
                                       IPreviewPageExtractorProvider,
                                       IWeakEventListener
    {
        public string FileNameColumnName { get; }

        /// <summary>
        ///     The component.
        /// </summary>
        [NotNull]
        private readonly IWorkplaceViewComponent component;

        /// <summary>
        /// The data template func.
        /// </summary>
        [CanBeNull]
        private readonly Func<IPlaceArea, DataTemplate> dataTemplateFunc;

        /// <summary>
        ///     The preview page extractor provider.
        /// </summary>
        [CanBeNull]
        private readonly IPreviewPageExtractorProvider previewPageExtractorProvider;

        /// <summary>
        ///     The preview handlers pool.
        /// </summary>
        [CanBeNull]
        private IPreviewHandlersPool previewHandlersPool;

        /// <summary>
        ///     The preview path.
        /// </summary>
        private string previewPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewView"/> class.
        /// </summary>
        /// <param name="previewHandlersPool">
        /// Пул обработчиков предварительного просмотра
        /// </param>
        /// <param name="previewPageExtractorProvider">
        /// Поставщик предоставляющий доступ к объекту осуществляющему
        ///     извлечение страниц для просмотра из многостраничных документов
        /// </param>
        /// <param name="component">
        /// Представление
        /// </param>
        /// <param name="fileNameColumnName">Имя столбца из которого будет браться путь к файлу</param>
        /// <param name="placeAreas">
        /// Область отображения
        /// </param>
        /// <param name="dataTemplateFunc">
        /// Функция возвращающая шаблон отображения по области отображения, по умолчанию <c>null</c>
        /// </param>
        public PreviewView(
            [CanBeNull] IPreviewHandlersPool previewHandlersPool,
            [CanBeNull] IPreviewPageExtractorProvider previewPageExtractorProvider,
            [NotNull] IWorkplaceViewComponent component,
            [CanBeNull] string fileNameColumnName = null,
            [CanBeNull] IEnumerable<IPlaceArea> placeAreas = null,
            [CanBeNull] Func<IPlaceArea, DataTemplate> dataTemplateFunc = null)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            
            this.InitializeComponent();
            this.previewHandlersPool = previewHandlersPool;
            this.previewPageExtractorProvider = previewPageExtractorProvider;
            this.component = component;
            this.dataTemplateFunc = dataTemplateFunc;
            this.PlaceAreas = placeAreas ?? ContentPlaceAreas.ContentPlaces;
            this.Order = 0;
            this.FileNameColumnName = fileNameColumnName;
            this.DataContext = this;
            PropertyChangedEventManager.AddHandler(this.component, this.DataChanged, "Data");

            SelectionStateChangedEventManager.AddListener(
                this.component.Selection,
                this);
            this.UpdateFilePath();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public int Order { get; }

        /// <inheritdoc />
        public IEnumerable<IPlaceArea> PlaceAreas { get; }

        /// <summary>
        ///     Gets or sets Пул обработчиков предварительного просмотра
        /// </summary>
        [CanBeNull]
        public IPreviewHandlersPool PreviewHandlersPool
        {
            get => this.previewHandlersPool;
            set
            {
                if (this.previewHandlersPool == value)
                {
                    return;
                }

                this.previewHandlersPool = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(PreviewView.PreviewHandlersPool)));
            }
        }

        /// <summary>
        ///     Gets or sets Путь к отображаемому файлу.
        /// </summary>
        public string PreviewPath
        {
            get => this.previewPath;
            set
            {
                if (this.previewPath == value)
                {
                    return;
                }

                this.previewPath = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(PreviewView.PreviewPath)));
            }
        }

        /// <inheritdoc />
        public DataTemplate GetTemplate(IPlaceArea area)
        {
            return this.dataTemplateFunc?.Invoke(area);
        }

        /// <inheritdoc />
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(SelectionStateChangedEventManager))
            {
                this.UpdateFilePath();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IPreviewPageExtractor TryGetPageExtractor(string filePath)
        {
            return this.previewPageExtractorProvider != null
                       ? this.previewPageExtractorProvider.TryGetPageExtractor(filePath)
                       : null;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Обрабатывает изменение строк данных
        /// </summary>
        /// <param name="sender">
        /// Источник события
        /// </param>
        /// <param name="e">
        /// Параметры события
        /// </param>
        private void DataChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateFilePath();
        }

        /// <summary>
        ///     Обновляет путь к файлу.
        /// </summary>
        private void UpdateFilePath()
        {
            IDictionary<string, object> row = this.component.SelectedRow ?? this.component.Data?.FirstOrDefault();
            if (row != null)
            {
                if (!string.IsNullOrWhiteSpace(this.FileNameColumnName) && row.TryGetValue(this.FileNameColumnName, out object filePath))
                {
                    this.PreviewPath = filePath as string;
                    return;
                }

                this.PreviewPath = row.Values.FirstOrDefault() as string;
                return;
            }

            this.PreviewPath = string.Empty;
        }
    }
}