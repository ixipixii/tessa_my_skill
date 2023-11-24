using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrPartnerRequestsTypes
    {
        public static readonly int CreateID = 0;
        public const string CreateName = "Создание нового контрагента";

        public static readonly int ApproveID = 1;
        public const string ApproveName = "Согласование контрагента";
    }
}