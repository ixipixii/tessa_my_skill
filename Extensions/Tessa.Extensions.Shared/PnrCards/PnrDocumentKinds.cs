using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrDocumentKinds
    {
        // По административно-хозяйственной деятельности
        public static readonly Guid OrderAdministrativeActivity = new Guid("B100BB1D-A2F1-40A0-9F6E-62C3F05AE365");

        // По основной деятельности
        public static readonly Guid OrderMainActivity = new Guid("79C4402F-6FD6-4F5C-9829-B8FBD4AE6C3E");

        // По реализации
        public static readonly Guid OrderImplementation = new Guid("102315FA-C955-4E83-AE33-8D7C127F2274");

        // По обеспечению корп.моб.связи
        public static readonly Guid OrderMobileCommunications = new Guid("42E6E6AF-C176-4B37-9490-44D4D256C028");

        // Распоряжение
        public static readonly Guid Disposal = new Guid("C73FD2C5-019A-41A4-BE2D-CA1508692C92");
    }
}
