using DocumentFormat.OpenXml;

namespace Tessa.Extensions.Default.Server.Cards
{
    public static class OpenXmlExtensions
    {
        /// <summary>
        /// Безопасное удаление элемента. Удаляет элемент в случае, если он прикреплен к родительскому элементу
        /// </summary>
        /// <param name="element">Удаляемый элемент</param>
        public static void SafeRemove(this OpenXmlElement element)
        {
            if (element.Parent != null)
            {
                element.Remove();
            }
        }

    }
}
