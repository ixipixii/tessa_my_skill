// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerWorkplaceExtension.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces.Manager
{
    #region

    using System;

    using Tessa.Properties.Resharper;
    using Tessa.UI;
    using Tessa.UI.Views;
    using Tessa.UI.Views.Extensions;
    using Unity;
    using Tessa.Views;
    using Tessa.Extensions.Default.Shared.Workplaces;

    #endregion

    /// <summary>
    ///     Расширение рабочего места руководителя
    /// </summary>
    public sealed class ManagerWorkplaceExtension : IWorkplaceViewComponentExtension, IWorkplaceExtensionSettingsRestore
    {
        /// <summary>
        /// The container.
        /// </summary>
        [NotNull]
        private readonly IUnityContainer container;

        [NotNull]
        private readonly ImageCache imageCache;

        /// <summary>
        /// The settings.
        /// </summary>
        [NotNull]
        private ManagerWorkplaceSettings settings;

        /// <inheritdoc />
        public ManagerWorkplaceExtension([NotNull] IUnityContainer container, [NotNull] ImageCache imageCache)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (imageCache == null)
            {
                throw new ArgumentNullException("imageCache");
            }

            this.container = container;
            this.imageCache = imageCache;
        }

        /// <inheritdoc />
        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        /// <inheritdoc />
        public void Initialize(IWorkplaceViewComponent model)
        {
            model.ContentFactories[StandardViewComponentContentItemFactory.Table] =
                c => new ManagerWorkplaceContentItem(c, this.imageCache, this.settings);
        }

        /// <inheritdoc />
        public void Initialized(IWorkplaceViewComponent model)
        {
        }

        /// <summary>
        /// Осуществляет восстановление настроек расширения
        /// </summary>
        /// <param name="metadata">
        /// Сериализованные метаданные настроек
        /// </param>
        public void Restore(byte[] metadata)
        {
            this.settings = ExtensionSettingsSerializationHelper.Deserialize<ManagerWorkplaceSettings>(metadata);
        }
    }
}