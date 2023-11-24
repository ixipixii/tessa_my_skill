using System;
using System.IO;
using Tessa.UI.Files;

namespace Tessa.Extensions.Default.Client.Files
{
    /// <summary>
    /// Объект, выполняющий извлечение страницы для предпросмотра из многостраничного документа,
    /// тип которого определяется автоматически по расширению.
    /// </summary>
    public sealed class DefaultPreviewPageExtractor :
        IPreviewPageExtractor
    {
        #region Constructors

        public DefaultPreviewPageExtractor(
            Func<PdfPreviewPageExtractor> getPdfFunc,
            Func<TiffPreviewPageExtractor> getTiffFunc)
        {
            this.getPdfFunc = getPdfFunc;
            this.getTiffFunc = getTiffFunc;
        }

        #endregion

        #region Fields

        private readonly Func<PdfPreviewPageExtractor> getPdfFunc;

        private readonly Func<TiffPreviewPageExtractor> getTiffFunc;

        #endregion

        #region IPreviewPageExtractor Members

        /// <summary>
        /// Выполняет извлечение страницы для предпросмотра.
        /// </summary>
        /// <param name="context">
        /// Контекст, содержащий параметры извлечения. В этот объект должен быть записан результат извлечения.
        /// </param>
        public void ExtractPage(IPreviewPageExtractorContext context)
        {
            string extension = Path.GetExtension(context.FilePath);
            if (string.IsNullOrEmpty(extension))
            {
                throw new InvalidOperationException(
                    "File without extension can't be previewed by " + this.GetType().FullName);
            }

            extension = extension.ToLowerInvariant();

            switch (extension)
            {
                case ".pdf":
                    this.getPdfFunc().ExtractPage(context);
                    return;

                case ".tif":
                case ".tiff":
                    this.getTiffFunc().ExtractPage(context);
                    return;

                default:
                    throw new InvalidOperationException(
                        "File with extension + " + extension + " can't be previewed by " + this.GetType().FullName);
            }
        }

        #endregion
    }
}
