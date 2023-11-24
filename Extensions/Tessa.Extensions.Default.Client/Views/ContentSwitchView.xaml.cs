using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.UI.Views;
using Tessa.UI.Views.Content;

namespace Tessa.Extensions.Default.Client.Views
{
    /// <summary>
    ///     Представление отображеющее кнопки переключения режима  <see cref="ContentViewMode" /> отображения данных модели
    ///     представления <see cref="IWorkplaceViewComponent" />
    /// </summary>
    public partial class ContentSwitchView : IContentItem, INotifyPropertyChanged
    {
        /// <summary>
        /// The icon container factory.
        /// </summary>
        private readonly Func<IIconContainer> iconContainerFactory;

        /// <summary>
        ///     The model.
        /// </summary>
        private readonly IWorkplaceViewComponent model;

        private readonly TableViewFactory tableViewFactory;

        /// <summary>
        ///     The content view mode.
        /// </summary>
        private ContentViewMode contentViewMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSwitchView"/> class.
        /// </summary>
        /// <param name="model">
        /// Модель представления рабочего места
        /// </param>
        /// <param name="tableViewFactory"></param>
        /// <param name="iconContainerFactory">
        /// Фабрика получения контейнера значков приложения
        /// </param>
        /// <param name="contentViewMode">
        /// Режим отображения контента
        /// </param>
        /// <param name="placeAreas">
        /// Области отображения данного представления
        /// </param>
        /// <param name="ordering">
        /// Порядок вывода
        /// </param>
        public ContentSwitchView(
            IWorkplaceViewComponent model,
            TableViewFactory tableViewFactory,
            Func<IIconContainer> iconContainerFactory,
            ContentViewMode contentViewMode = ContentViewMode.TableView,
            IEnumerable<IPlaceArea> placeAreas = null,
            int ordering = PlacementOrdering.BeforeAll)
        {
            this.InitializeComponent();
            this.model = model;
            this.tableViewFactory = tableViewFactory;
            this.iconContainerFactory = iconContainerFactory;
            this.contentViewMode = contentViewMode;
            this.Order = ordering;
            this.PlaceAreas = placeAreas ?? ContentPlaceAreas.ToolbarPlaces;
            this.DataContext = this;
        }

        /// <summary>
        ///     The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets or sets Режим отображения контента
        /// </summary>
        public ContentViewMode ContentViewMode
        {
            get
            {
                return this.contentViewMode;
            }

            set
            {
                if (this.contentViewMode == value)
                {
                    return;
                }

                this.contentViewMode = value;
                this.OnPropertyChanged("ContentViewMode");
                this.OnPropertyChanged("RecordMode");
                this.OnPropertyChanged("TableMode");
                this.UpdateContent();
            }
        }

        /// <summary>
        ///     Gets Порядок вывода элемента
        /// </summary>
        public int Order { get; }

        /// <summary>
        ///     Gets возвращает область отображения
        /// </summary>
        public IEnumerable<IPlaceArea> PlaceAreas { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether Признак отображения строк
        /// </summary>
        public bool RecordMode
        {
            get
            {
                return this.contentViewMode == ContentViewMode.RecordView;
            }

            set
            {
                this.ContentViewMode = !value ? ContentViewMode.TableView : ContentViewMode.RecordView;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether Признак отображения таблицы
        /// </summary>
        public bool TableMode
        {
            get
            {
                return this.contentViewMode == ContentViewMode.TableView;
            }

            set
            {
                this.ContentViewMode = value ? ContentViewMode.TableView : ContentViewMode.RecordView;
            }
        }

        /// <summary>
        /// Возвращает шаблон данных для указанной области
        /// </summary>
        /// <param name="area">
        /// Область расположения
        /// </param>
        /// <returns>
        /// Шаблон данных или null
        /// </returns>
        public DataTemplate GetTemplate(IPlaceArea area)
        {
            return null;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The delete content item.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        private void DeleteContentItem(Type type)
        {
            var contentItem = this.model.Content.FirstOrDefault(c => c.GetType() == type);
            if (contentItem != null)
            {
                this.model.Content.Remove(contentItem);

                switch (contentItem)
                {
                    case IAsyncDisposable asyncDisposable:
                        asyncDisposable.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult(); // TODO async
                        break;

                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        /// <summary>
        ///     The update content.
        /// </summary>
        private void UpdateContent()
        {
            if (this.TableMode)
            {
                this.DeleteContentItem(typeof(RecordView));
                this.model.ContentFactories[StandardViewComponentContentItemFactory.Table] =
                    c => this.tableViewFactory(c, ContentPlaceAreas.ContentPlaces, null, PlacementOrdering.BeforeAll);//new ViewContentItem(c, this.iconContainerFactory);
                this.model.Content.Add(this.tableViewFactory(this.model, ContentPlaceAreas.ContentPlaces, null, PlacementOrdering.BeforeAll));
                return;
            }

            if (this.RecordMode)
            {
                this.DeleteContentItem(typeof(TableView));
                this.model.ContentFactories[StandardViewComponentContentItemFactory.Table] = c => new RecordView(c);
                this.model.Content.Add(new RecordView(this.model));
            }
        }
    }
}