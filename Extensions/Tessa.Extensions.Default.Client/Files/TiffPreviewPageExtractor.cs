using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tessa.UI.Files;

namespace Tessa.Extensions.Default.Client.Files
{
    /// <summary>
    /// Объект, выполняющий извлечение страницы для предпросмотра из многостраничного документа TIFF.
    /// </summary>
    public sealed class TiffPreviewPageExtractor :
        IPreviewPageExtractor
    {
        #region IPreviewPageExtractor Members

        /// <summary>
        /// Выполняет извлечение страницы для предпросмотра.
        /// </summary>
        /// <param name="context">
        /// Контекст, содержащий параметры извлечения. В этот объект должен быть записан результат извлечения.
        /// </param>
        public void ExtractPage(IPreviewPageExtractorContext context)
        {
            // используем файл в формате Bmp, т.к. методом случайного научного выбора было определено, что это быстрее всего

            using Image image = Image.FromFile(context.FilePath);
            context.ResultPageCount = image.GetFrameCount(FrameDimension.Page);
            image.SelectActiveFrame(FrameDimension.Page, context.PageIndex);

            // по умолчанию используем Stream размером меньше 80 Кб
            using var memoryStream = new MemoryStream(capacity: 80000);
            image.Save(memoryStream, ImageFormat.Bmp);
            image.Dispose();

            context.ResultImageBytes = memoryStream.ToArray();
        }

        #endregion
    }
}
