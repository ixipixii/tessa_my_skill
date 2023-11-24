// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewExtension.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using Tessa.Platform;
    using Tessa.PreviewHandlers;
    using Tessa.UI.Files;
    using Tessa.UI.Views;

    using Unity;

    /// <summary>
    /// Расширение позволяющее отображать содержимое файла по пути к нему
    /// </summary>
    public sealed class PreviewExtension : IWorkplaceViewComponentExtension, IPreviewPageExtractorProvider
    {
        /// <summary>
        /// The preview handlers pool.
        /// </summary>
        private readonly IPreviewHandlersPool previewHandlersPool;

        /// <summary>
        /// The unity container.
        /// </summary>
        private readonly IUnityContainer unityContainer;

        /// <inheritdoc />
        public PreviewExtension(IPreviewHandlersPool previewHandlersPool, IUnityContainer unityContainer)
        {
            this.previewHandlersPool = previewHandlersPool;
            this.unityContainer = unityContainer;
        }

        /// <inheritdoc />
        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        /// <inheritdoc />
        public void Initialize(IWorkplaceViewComponent model)
        {
            model.ContentFactories.Clear();
            model.ContentFactories["Preview"] = c => new PreviewView(this.previewHandlersPool, this, c);
        }

        /// <inheritdoc />
        public void Initialized(IWorkplaceViewComponent model)
        {
        }

        /// <inheritdoc />
        public IPreviewPageExtractor TryGetPageExtractor(string filePath)
        {
            return this.unityContainer.TryResolve<IPreviewPageExtractor>();
        }
    }
}