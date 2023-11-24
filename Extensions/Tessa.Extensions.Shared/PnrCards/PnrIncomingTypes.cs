using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrIncomingTypes
    {
        public static readonly Guid IncomingLetterID = new Guid("8a58a6be-a235-43b4-bc2b-648a42799895");
        public static readonly string IncomingLetterIdx = "1-01";
        public static readonly string IncomingLetterName = "Входящее письмо";

        public static readonly Guid IncomingComplaintsID = new Guid("a7e9340b-70f6-46b1-8049-00d9ec91e910");
        public static readonly string IncomingComplaintsIdx = "1-02";
        public static readonly string IncomingComplaintsName = "Рекламации";
    }
}
