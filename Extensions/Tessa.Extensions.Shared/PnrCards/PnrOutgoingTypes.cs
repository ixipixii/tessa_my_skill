using System;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrOutgoingTypes
    {
        public static readonly Guid OutgoingLetterID = new Guid("f5d461c5-2237-4e68-b500-de6b17fc6b38");
        public static readonly string OutgoingLetterIdx = "2-01";
        public static readonly string OutgoingLetterName = "Исходящее письмо";

        public static readonly Guid OutgoingComplaintsID = new Guid("9eab450e-6c37-4542-8d0a-2da0f91c80c4");
        public static readonly string OutgoingComplaintsIdx = "2-02";
        public static readonly string OutgoingComplaintsName = "Ответы на рекламации";
    }
}
