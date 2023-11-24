// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutomaticNodeRefreshConfigurator.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces
{
    using System;
    using System.IO;

    using Tessa.Json;
    using Tessa.Json.Bson;
    using Tessa.Platform.Json;
    using Tessa.UI.Views.Extensions;
    using Tessa.UI.Windows;
    using Tessa.Extensions.Default.Shared.Workplaces;

    /// <summary>
    ///     Конфигуратор для расширения <see cref="AutomaticNodeRefreshExtension" />
    /// </summary>
    public class AutomaticNodeRefreshConfigurator : IExtensionSettingsConfigurator
    {
        /// <inheritdoc />
        public void Configure(IExtensionConfigurationContext context)
        {
            byte[] settings = context.GetSettings();
            var model = settings == null ? new AutomaticNodeRefreshSettings() : this.Load(settings);
            var viewModel = new AutomaticRefreshViewModel(model);
            var window = new AutomaticNodeRefreshSettingsWindow();
            window.CloseOnPreviewMiddleButtonDown();
            window.DataContext = viewModel;
            if (window.ShowDialog() == true)
            {
                SaveSettings(context, model);
            }
        }

        /// <inheritdoc />
        public void Finalize(IExtensionConfigurationContext context)
        {
        }

        /// <inheritdoc />
        public void Initialize(IExtensionConfigurationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var model = new AutomaticNodeRefreshSettings();
            SaveSettings(context, model);
        }

        /// <summary>
        /// The save settings.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        private static void SaveSettings(IExtensionConfigurationContext context, IAutomaticNodeRefreshSettings model)
        {
            using var stream = new MemoryStream();
            using (var writer = new BsonWriter(stream) { CloseOutput = false })
            {
                var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                serializer.Converters.Add(new GuidConverter());
                serializer.Converters.Add(new SchemeTypeConverter());
                serializer.Serialize(writer, model);
            }

            stream.Position = 0;
            context.SaveSettings(stream.ToArray());
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="IAutomaticNodeRefreshSettings"/>.
        /// </returns>
        private IAutomaticNodeRefreshSettings Load(byte[] settings)
        {
            using var stream = new MemoryStream(settings);
            using var bsonReader = new BsonReader(stream);
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.All }.Deserialize<IAutomaticNodeRefreshSettings>(bsonReader);
        }
    }
}