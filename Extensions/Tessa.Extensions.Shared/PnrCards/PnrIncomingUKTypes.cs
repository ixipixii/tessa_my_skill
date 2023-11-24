using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrIncomingUKTypes
    {
        public static readonly Guid IncomingUKLetterID = new Guid("a215d9e5-1ad5-435e-8930-641e68e0ce90");
        public static readonly string IncomingUKLetterIdx = "0-01";
        public static readonly string IncomingUKLetterName = "Входящее письмо УК ПС";

        public static readonly Guid IncomingUKComplaintsID = new Guid("1610d1c9-63d7-445d-b972-a498ae01ff42");
        public static readonly string IncomingUKComplaintsIdx = "0-02";
        public static readonly string IncomingUKComplaintsName = "Рекламация УК ПС";
    }
}
