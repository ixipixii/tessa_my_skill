using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tessa.PdfiumViewer;

namespace Tessa.Extensions.Default.Client.Pdf
{
    /// <summary>
    /// Метаинформация по документу PDF.
    /// </summary>
    public sealed class PdfiumDocumentMetadata
    {
        #region Constructors

        public PdfiumDocumentMetadata(PdfDocument document)
        {
            object pdfFile = fileField.GetValue(document);
            this.handle = (IntPtr)documentField.GetValue(pdfFile);
        }

        #endregion

        #region Fields

        private readonly IntPtr handle;

        private readonly Dictionary<string, string> cachedMetadata =
            new Dictionary<string, string>(StringComparer.Ordinal);

        private static readonly FieldInfo fileField = typeof(PdfDocument)
            .GetField("_file", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo documentField = typeof(PdfDocument)
            .Assembly
            .GetType("Tessa.PdfiumViewer.PdfFile", throwOnError: true)
            .GetField("_document", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly object syncObject = typeof(PdfDocument)
            .Assembly
            .GetType("Tessa.PdfiumViewer.NativeMethods", throwOnError: true)
            .GetField("LockString", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null);

        #endregion

        #region Private Methods

        private string GetValue(string key)
        {
            if (!this.cachedMetadata.TryGetValue(key, out string value))
            {
                value = FPDF_GetMetaText(this.handle, "Author");
                this.cachedMetadata[key] = value;
            }

            return value;
        }

        private static string FPDF_GetMetaText(IntPtr document, string tag)
        {
            lock (syncObject)
            {
                int code = PdfNativeMethods.FPDF_GetMetaText(document, tag, null, 0);
                if (code == 0)
                {
                    return string.Empty;
                }

                byte[] data = new byte[code];
                PdfNativeMethods.FPDF_GetMetaText(document, tag, data, code);

                return Encoding.Unicode.GetString(data).Trim('\0');
            }
        }

        #endregion

        #region Properties

        public string Title => this.GetValue("Title");

        public string Author => this.GetValue("Author");

        public string Subject => this.GetValue("Subject");

        public string Keywords => this.GetValue("Keywords");

        public string Creator => this.GetValue("Creator");

        public string Producer => this.GetValue("Producer");

        public string CreationDate => this.GetValue("CreationDate");

        public string ModificationDate => this.GetValue("ModDate");

        public string Trapped => this.GetValue("Trapped");

        #endregion
    }
}
