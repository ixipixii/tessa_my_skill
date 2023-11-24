// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerWorkplaceExtensionConfigurator.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces.Manager
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Tessa.Properties.Resharper;
    using Tessa.UI.Views;
    using Tessa.UI.Views.Content;
    using Tessa.UI.Views.Extensions;
    using Unity;
    using Tessa.Views;
    using Tessa.Views.Metadata;
    using Tessa.Extensions.Default.Shared.Workplaces;

    #endregion

    /// <summary>
    ///     Конфигуратор для рабочего места руководителя
    /// </summary>
    public sealed class ManagerWorkplaceExtensionConfigurator : IExtensionSettingsConfigurator
    {
        /// <summary>
        ///     The container.
        /// </summary>
        [NotNull]
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerWorkplaceExtensionConfigurator"/> class.
        /// </summary>
        /// <param name="container">
        /// Контейнер приложения
        /// </param>
        public ManagerWorkplaceExtensionConfigurator([NotNull] IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        /// <summary>
        /// Вызывается для конфигурации расширения
        /// </summary>
        /// <param name="context">
        /// Контекст конфигурации расширения
        /// </param>
        public void Configure(IExtensionConfigurationContext context)
        {
            using IUnityContainer localContainer = this.container.CreateChildContainer();
            IDataSourceMetadata dataSourceMetadata = context.GetPropertyValue<IDataSourceMetadata>("Metadata");
            IWorkplaceViewComponent item = this.GetViewComponent(dataSourceMetadata, localContainer);
            IEnumerable<string> columnNames = Enumerable.Empty<string>();
            if (item != null)
            {
                var metadata = item.GetViewMetadata(item);
                if (metadata != null)
                {
                    columnNames = metadata.Columns.Select(c => c.Alias).ToArray();
                }
            }

            var settings = this.LoadOrCreate(context, localContainer);
            var settingsViewModel = SettingsViewModel.Create(settings, columnNames);
            var settingsDialog = new ManagerSettingsDialog();
            settingsDialog.DataContext = settingsViewModel;
            if (settingsDialog.ShowDialog() != true)
            {
                return;
            }

            settings.ActiveImageColumnName = settingsViewModel.ActiveImageColumnName;
            settings.CardId = settingsViewModel.CardId;
            settings.HoverImageColumnName = settingsViewModel.HoverImageColumnName;
            settings.InactiveImageColumnName = settingsViewModel.InactiveImageColumnName;
            settings.CountColumnName = settingsViewModel.CountColumnName;
            settings.TileColumnName = settingsViewModel.TileColumnName;
            byte[] bytes = ExtensionSettingsSerializationHelper.Serialize(settings);
            context.SaveSettings(bytes);
            context.ResetViewComponent();
        }

        /// <summary>
        /// Вызывается при удалении расширения из контейнера
        /// </summary>
        /// <param name="context">
        /// Контекст конфигурации расширения
        /// </param>
        public void Finalize(IExtensionConfigurationContext context)
        {
        }

        /// <summary>
        /// Вызывается для инициализации расширения после добавления его в контейнер
        /// </summary>
        /// <param name="context">
        /// Контекст инициализации
        /// </param>
        public void Initialize(IExtensionConfigurationContext context)
        {
            ManagerWorkplaceSettings settingsModel = new ManagerWorkplaceSettings
            {
                CardId = new Guid("3db19fa0-228a-497f-873a-0250bf0a4ccb"),
                TileColumnName = "Caption",
                CountColumnName = "Count",
                ActiveImageColumnName = "ActiveImage",
                HoverImageColumnName = "ActiveImage",
                InactiveImageColumnName = "InactiveImage",
            };

            byte[] bytes = ExtensionSettingsSerializationHelper.Serialize(settingsModel);
            context.SaveSettings(bytes);
        }

        /// <summary>
        /// Создает и возвращает Компонент отображения данных представления
        /// </summary>
        /// <param name="dataSourceMetadata">
        /// Метаданные источника данных
        /// </param>
        /// <param name="container">
        /// Контейнер
        /// </param>
        /// <returns>
        /// Компонент отображения данных
        /// </returns>
        [CanBeNull]
        private IWorkplaceViewComponent GetViewComponent(
            [NotNull] IDataSourceMetadata dataSourceMetadata,
            [NotNull] IUnityContainer container)
        {
            Contract.Requires(container != null);
            Contract.Requires(dataSourceMetadata != null);

            return WorkplaceViewComponentHelper.CreateWorkplaceViewComponent(
                container.Resolve<ContentFactory>(),
                dataSourceMetadata,
                null,
                new List<RequestParameter>(),
                new Dictionary<string, IEnumerable<RequestParameter>>(StringComparer.OrdinalIgnoreCase),
                container.Resolve<IWorkplaceExtensionExecutorFactory>());
        }

        /// <summary>
        /// Загружает при наличии сохраненных настроек или создает модель для
        ///     настроек рабочего места руководителя по-умолчанию.
        /// </summary>
        /// <param name="context">
        /// Контекст редактирования расширения
        /// </param>
        /// <param name="container">
        /// Контейнер приложения
        /// </param>
        /// <exception cref="ApplicationException">
        /// Ошибка загрузки или создания диаграммы
        /// </exception>
        /// <returns>
        /// Модель настроек рабочего места руководителя
        /// </returns>
        [NotNull]
        private ManagerWorkplaceSettings LoadOrCreate(
            [NotNull] IExtensionConfigurationContext context,
            [NotNull] IUnityContainer container)
        {
            Contract.Requires(context != null);
            byte[] settings = context.GetSettings();
            return settings == null
                ? new ManagerWorkplaceSettings()
                : ExtensionSettingsSerializationHelper.Deserialize<ManagerWorkplaceSettings>(settings);
        }
    }
}