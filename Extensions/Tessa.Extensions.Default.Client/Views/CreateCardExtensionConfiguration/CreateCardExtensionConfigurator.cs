// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCardExtensionConfigurator.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using System.Diagnostics.Contracts;

    using Tessa.Extensions.Default.Shared.Views;
    using Tessa.Properties.Resharper;
    using Tessa.Views;
    using Tessa.UI.Views.Extensions;
    using Tessa.UI.Windows;

    /// <summary>
    ///     Предоставляет возможность настройки расширения <see cref="CreateCardExtension" />
    /// </summary>
    public sealed class CreateCardExtensionConfigurator : IExtensionSettingsConfigurator
    {
        /// <inheritdoc />
        public void Configure(IExtensionConfigurationContext context)
        {
            CreateCardExtensionSettings settings = LoadOrCreate(context);
            CreateCardExtensionSettingsViewModel settingsViewModel = CreateCardExtensionSettingsViewModel.Create(settings);

            CreateCardExtensionSettingsDialog settingsDialog = new CreateCardExtensionSettingsDialog { DataContext = settingsViewModel };
            settingsDialog.CloseOnPreviewMiddleButtonDown();

            if (settingsDialog.ShowDialog() == true)
            {
                settingsViewModel.ApplyTo(settings);

                byte[] bytes = ExtensionSettingsSerializationHelper.Serialize(settings);
                context.SaveSettings(bytes);
                context.ResetViewComponent();
            }
        }

        /// <inheritdoc />
        public void Finalize(IExtensionConfigurationContext context)
        {
        }

        /// <inheritdoc />
        public void Initialize(IExtensionConfigurationContext context)
        {
            CreateCardExtensionSettings settingsModel = new CreateCardExtensionSettings();
            byte[] bytes = ExtensionSettingsSerializationHelper.Serialize(settingsModel);
            context.SaveSettings(bytes);
        }

        /// <summary>
        /// The load or create.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="CreateCardExtensionSettings"/>.
        /// </returns>
        private static CreateCardExtensionSettings LoadOrCreate([NotNull] IExtensionConfigurationContext context)
        {
            Contract.Requires(context != null);
            byte[] settings = context.GetSettings();
            return settings == null
                       ? new CreateCardExtensionSettings()
                       : ExtensionSettingsSerializationHelper.Deserialize<CreateCardExtensionSettings>(settings);
        }
    }
}