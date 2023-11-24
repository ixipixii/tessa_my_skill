using System;
using Tessa.Localization;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Client.Pdf
{
    /// <summary>
    /// Вспомогательные методы-расширения для пространства имён <c>Tessa.Extensions.Platform.Client.Models.PdfStamps</c>.
    /// </summary>
    public static class PdfStampExtensions
    {
        #region IExtensionContainer Extensions

        public static IExtensionContainer RegisterPdfStampExtensionTypes(this IExtensionContainer extensionContainer)
        {
            return extensionContainer
                .RegisterType<IPdfStampExtension>(x => x
                    .MethodAsync<IPdfStampExtensionContext>(y => y.OnGenerationStarted)
                    .MethodAsync<IPdfStampExtensionContext>(y => y.GenerateForPage)
                    .MethodAsync<IPdfStampExtensionContext>(y => y.OnGenerationEnded))
                ;
        }

        #endregion

        #region PdfStampWriter Extensions

        /// <summary>
        /// Добавляет строку с заданной датой в штамп. Дата не конвертируется в локальное время и выводится как есть.
        /// </summary>
        /// <param name="stampWriter">Объект, выполняющий вывод штампа на странице PDF.</param>
        /// <param name="date">
        /// Выводимая дата (время и часовой пояс игнорируются) или <c>null</c>, если дата неизвестна.
        /// </param>
        /// <returns>Объект <paramref name="stampWriter"/> для цепочки вызовов.</returns>
        public static PdfStampWriter AppendDate(this PdfStampWriter stampWriter, DateTime? date)
        {
            if (stampWriter == null)
            {
                throw new ArgumentNullException("stampWriter");
            }

            return stampWriter
                .AppendLine(
                    LocalizationManager.Format(
                        "$UI_Controls_Scan_DateStamp",
                        FormattingHelper.FormatDate(date, convertToLocal: false)));
        }

        #endregion
    }
}
