using System;

namespace Tessa.Extensions.Default.Shared.VirtualFiles
{
    public static class KrVirtualFilesHelper
    {
        /// <summary>
        /// ID виртуального файла листа согласования
        /// </summary>
        public static Guid ApprovalListFileID = new Guid("6e69fc8d-8c1d-4ca4-bf59-0dbae4e21420");

        /// <summary>
        /// ID основной версии виртуального файла листа согласования
        /// </summary>
        public static Guid ApprovalListDefaultVersionID = new Guid("9b299233-1e28-490d-a009-714f8b0ba74f");

        /// <summary>
        /// ID печатной версии виртуального файла листа согласования
        /// </summary>
        public static Guid ApprovalListPrintableVersionID = new Guid("f6f565cc-4f6f-4d3b-98c3-ec59bf0d846f");
    }
}
