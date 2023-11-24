using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrContractForms
    {
        // Форма договора
        public static readonly Guid PnrAtypicalID = new Guid("547ad4da-cb7f-478d-8b17-9746cc81ce01");
        public static readonly string PnrAtypicalName = "Нетиповой";

        public static readonly Guid PnrWithMonopolistID = new Guid("933afcc6-865d-4951-b5e6-2f73de05c65c");
        public static readonly string PnrWithMonopolistName = "С монополистом";

        public static readonly Guid PnrTypicalID = new Guid("e8550e9e-b25c-4602-8590-8aec29f2b6b1");
        public static readonly string PnrTypicalName = "Типовой";
    }
}
