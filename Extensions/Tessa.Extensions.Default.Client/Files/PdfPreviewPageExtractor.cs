using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tessa.PdfiumViewer;
using Tessa.UI;
using Tessa.UI.Files;

namespace Tessa.Extensions.Default.Client.Files
{
    /// <summary>
    /// Объект, выполняющий извлечение страницы для предпросмотра из многостраничного документа PDF.
    /// </summary>
    public sealed class PdfPreviewPageExtractor :
        IPreviewPageExtractor
    {
        // любые зависимости Unity можно получить через конструктор

        #region IPreviewPageExtractor Members

        /// <summary>
        /// Выполняет извлечение страницы для предпросмотра.
        /// </summary>
        /// <param name="context">
        /// Контекст, содержащий параметры извлечения. В этот объект должен быть записан результат извлечения.
        /// </param>
        public void ExtractPage(IPreviewPageExtractorContext context)
        {
            byte[] imageBytes;
            using (var document = PdfDocument.Load(context.FilePath))
            {
                context.ResultPageCount = document.PageCount;

                const int dpiTarget = 150;
                const double pdfDpiDefault = 72d;

                double systemDpiX = DpiHelpers.DeviceDpiX;
                double cX = systemDpiX / pdfDpiDefault;
                int dpiX = (int)(dpiTarget * cX);
                double factorX = dpiX / systemDpiX;

                double systemDpiY = DpiHelpers.DeviceDpiY;
                double cY = systemDpiY / pdfDpiDefault;
                int dpiY = (int)(dpiTarget * cY);
                double factorY = dpiY / systemDpiY;

                IList<SizeF> pageSizes = document.PageSizes;
                SizeF size = pageSizes[context.PageIndex];
                int pageWidthPixels = (int)size.Width;
                int pageHeightPixels = (int)size.Height;

                int renderWidthPixels = (int)Math.Round(pageWidthPixels * factorX, 0, MidpointRounding.AwayFromZero);
                int renderHeightPixels = (int)Math.Round(pageHeightPixels * factorY, 0, MidpointRounding.AwayFromZero);

                int width;
                int height;

                switch (context.Quality)
                {
                    case PreviewPageQuality.Low:
                        width = renderWidthPixels / 2;
                        height = renderHeightPixels / 2;
                        break;

                    case PreviewPageQuality.Normal:
                        width = renderWidthPixels;
                        height = renderHeightPixels;
                        break;

                    case PreviewPageQuality.High:
                        width = renderWidthPixels * 2;
                        height = renderHeightPixels * 2;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(typeof(PreviewPageQuality).Name);
                }

                using Image image = document.Render(context.PageIndex, width, height, dpiX, dpiY, PdfRenderFlags.Annotations | PdfRenderFlags.LcdText);
                using var memoryStream = new MemoryStream(capacity: 84999);
                image.Save(memoryStream, ImageFormat.Png);
                imageBytes = memoryStream.ToArray();
            }

            context.ResultImageBytes = imageBytes;
        }

        #endregion
    }
}
