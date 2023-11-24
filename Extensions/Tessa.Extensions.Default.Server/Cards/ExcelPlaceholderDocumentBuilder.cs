using System;
using System.IO;
using Tessa.Platform.Placeholders;

namespace Tessa.Extensions.Default.Server.Cards
{
    public static class ExcelPlaceholderDocumentBuilder
    {
        public static IPlaceholderDocument Build(
            Guid templateCardID,
            MemoryStream documentStream,
            out Func<IPlaceholderDocument, byte[]> getDocumentContentFunc)
        {
            var document = new ExcelPlaceholderDocument(documentStream, templateCardID);
            getDocumentContentFunc = x => ((ExcelPlaceholderDocument)x).Stream.ToArray();

            return document;
        }
    }
}
