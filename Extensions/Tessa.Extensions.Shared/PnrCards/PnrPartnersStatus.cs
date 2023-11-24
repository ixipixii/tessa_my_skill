using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrPartnersStatus
    {
        public static readonly int AgreedID = 0;
        public const string AgreedName = "Согласован";

        public static readonly int NotAgreedID = 1;
        public const string NotAgreedName = "Не согласован";

        public static readonly int BlacklistedID = 2;
        public const string BlacklistedName = "В черном списке";
    }
}
