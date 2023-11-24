#region Usings

using System;
using System.IO;
using Tessa.Json;
using Tessa.Json.Bson;
using Tessa.Properties.Resharper;
using Tessa.UI.Views.Extensions;
using Tessa.UI.Windows;
using Tessa.Extensions.Default.Shared.Workplaces;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    /// <inheritdoc />
    [UsedImplicitly]
    public class RefSectionExtensionConfigurator : IExtensionSettingsConfigurator
    {
        #region Public methods and operators

        /// <inheritdoc />
        public void Configure(IExtensionConfigurationContext context)
        {
            byte[] settings = context.GetSettings();
            var model = settings == null ? new TreeItemFilteringSettings() : this.Load(settings);
            var viewModel = new TreeItemFilteringSettingsViewModel(model);
            var window = new WorkplaceFilteringSettingsWindow();
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

            SaveSettings(context, new TreeItemFilteringSettings());
        }

        #endregion

        #region Other methods

        private static void SaveSettings(IExtensionConfigurationContext context, ITreeItemFilteringSettings model)
        {
            using var stream = new MemoryStream();
            using (var writer = new BsonWriter(stream) { CloseOutput = false })
            {
                var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                serializer.Serialize(writer, model);
            }

            stream.Position = 0;
            context.SaveSettings(stream.ToArray());
        }

        private ITreeItemFilteringSettings Load(byte[] settings)
        {
            using var stream = new MemoryStream(settings);
            using var bsonReader = new BsonReader(stream);
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                .Deserialize<ITreeItemFilteringSettings>(bsonReader);
        }

        #endregion
    }
}