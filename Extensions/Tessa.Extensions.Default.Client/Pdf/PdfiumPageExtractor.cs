﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Extensions.Platform.Client.Scanning;
using Tessa.PdfiumViewer;
using Tessa.Platform;
using Tessa.Platform.IO;
using Tessa.UI;

namespace Tessa.Extensions.Default.Client.Pdf
{
    /// <summary>
    /// Объект, выполняющий разбор файла PDF на страницы с изображениями PNG посредством библиотеки PdfiumViewer.
    /// </summary>
    public class PdfiumPageExtractor :
        IPdfPageExtractor
    {
        // любые зависимости Unity можно получить через конструктор

        #region Properties

        /// <summary>
        /// Функция, которая загружает документ, используемый для разбора. Для него вызывается <see cref="IDisposable.Dispose"/>,
        /// а также он может быть одновременно открыт из нескольких потоков.
        /// Указано значение <c>null</c>, если создаётся новый документ для заданного имени файла.
        /// </summary>
        public Func<CancellationToken, ValueTask<PdfDocument>> LoadDocumentFuncAsync { get; set; }

        #endregion

        #region IPdfPageExtractor Members

        /// <summary>
        /// Выполняет извлечение страниц PDF с изображениями PNG.
        /// </summary>
        /// <param name="context">Контекст операции по разбору файла PDF на страницы.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public virtual async Task ExtractPagesAsync(IPdfPageExtractorContext context, CancellationToken cancellationToken = default)
        {
            Func<CancellationToken, ValueTask<PdfDocument>> loadDocumentFuncAsync = this.LoadDocumentFuncAsync
                ?? (ct => new ValueTask<PdfDocument>(PdfDocument.Load(context.PdfFilePath)));

            int pageCount;
            IList<SizeF> pageSizes;

            using (PdfDocument document = await loadDocumentFuncAsync(cancellationToken).ConfigureAwait(false))
            {
                pageCount = document.PageCount;
                pageSizes = document.PageSizes;
            }

            ITempFolder folder = TempFile.AcquireFolder();

            try
            {
                int[] convertedClosure = { 0 };

                ITempFile[] files = new ITempFile[pageCount];
                for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                {
                    files[pageIndex] = folder.AcquireFile(ScanDocumentHelper.GetPageFileName(pageIndex));
                }

                await Enumerable.Range(0, pageCount)
                    .RunWithMaxDegreeOfParallelismAsync(
                        UIHelper.MaxImageProcessingParallelThreads,
                        async (pageIndex, ct) =>
                        {
                            int converted = Interlocked.Increment(ref convertedClosure[0]);
                            await context.ReportProgressAsync(100.0 * converted / pageCount, ct).ConfigureAwait(false);

                            ITempFile tempFile = files[pageIndex];

                            int width = (int) pageSizes[pageIndex].Width * 3;
                            int height = (int) pageSizes[pageIndex].Height * 3;

                            using PdfDocument document = await loadDocumentFuncAsync(ct).ConfigureAwait(false);
                            using Image image = document.Render(pageIndex, width, height, 300, 300, PdfRenderFlags.ForPrinting);
                            image.Save(tempFile.Path, ImageFormat.Png);
                        },
                        cancellationToken).ConfigureAwait(false);

                for (int i = 0; i < files.Length; i++)
                {
                    context.PngPageFiles.Add(files[i]);
                }

                folder = null;
            }
            finally
            {
                folder?.Dispose();
            }
        }

        #endregion
    }
}