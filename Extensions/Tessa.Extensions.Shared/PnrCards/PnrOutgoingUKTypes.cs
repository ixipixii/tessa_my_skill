using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrOutgoingUKTypes
    {
        public static readonly Guid OutgoingUKLetterID = new Guid("2e1cf524-0d4a-435c-a956-617fb24d0381");
        public static readonly string OutgoingUKLetterIdx = "3-01";
        public static readonly string OutgoingUKLetterName = "Исходящее письмо УК ПС";

        public static readonly Guid OutgoingUKComplaintsID = new Guid("ca03ea3c-04e2-4890-85d2-d485ac598597");
        public static readonly string OutgoingUKComplaintsIdx = "3-02";
        public static readonly string OutgoingUKComplaintsName = "Ответы на рекламации УК ПС";
    }
}
