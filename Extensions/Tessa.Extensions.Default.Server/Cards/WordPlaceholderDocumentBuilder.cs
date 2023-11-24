using System;
using System.IO;
using Tessa.Platform.Placeholders;

namespace Tessa.Extensions.Default.Server.Cards
{
    public static class WordPlaceholderDocumentBuilder
    {
        public static IPlaceholderDocument Build(
            Guid templateCardID,
            MemoryStream documentStream,
            out Func<IPlaceholderDocument, byte[]> getDocumentContentFunc)
        {
            var document = new WordPlaceholderDocument(documentStream, templateCardID);
            getDocumentContentFunc = x => ((WordPlaceholderDocument)x).Stream.ToArray();

            return document;
        }
    }
}
