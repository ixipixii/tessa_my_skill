using System;
using System.Runtime.InteropServices;

namespace Tessa.Extensions.Default.Client.Pdf
{
    public static class PdfNativeMethods
    {
        #region Static Methods

        [DllImport("pdfium.dll", EntryPoint = "FPDF_GetMetaText", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FPDF_GetMetaText(
            IntPtr document,
            [MarshalAs(UnmanagedType.LPStr)] string tag,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
            int buflen);

        #endregion
    }
}
