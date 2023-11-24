using System;

namespace Tessa.Extensions.Default.Shared
{
    /// <summary>
    /// Уникальные идентификаторы обработчиков <see cref="Tessa.Platform.Initialization.IInitializationHandler"/>,
    /// используемых в типовом решении.
    /// </summary>
    public static class DefaultInitializationHandlers
    {
        #region Static Fields

        /// <summary>
        /// Информация по типам карточек и документов для типового решения.
        /// </summary>
        public static readonly Guid Types =             // F4C02A05-FBFB-4220-ADB3-5D81A14911BB
            new Guid(0xf4c02a05, 0xfbfb, 0x4220, 0xad, 0xb3, 0x5d, 0x81, 0xa1, 0x49, 0x11, 0xbb);

        #endregion
    }
}
