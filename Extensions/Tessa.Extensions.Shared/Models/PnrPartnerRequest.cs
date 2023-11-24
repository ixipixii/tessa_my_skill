using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    /// <summary>
    /// Заявка на контрагента
    /// </summary>
    public class PnrPartnerRequest
    {
        public Guid ID;

        public string FullNumber;

        public DateTime? CreationDate;

        public string StateName;

        public string ShortName;
        public string FullName;

        public int? RequestTypeID;
        public string RequestTypeName;

        public Guid? PartnerID;
        public string PartnerName;

        public string INN;
        public string KPP;
    }
}
