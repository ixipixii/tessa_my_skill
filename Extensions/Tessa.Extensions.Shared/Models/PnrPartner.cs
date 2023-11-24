using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    public class PnrPartner
    {
        public Guid ID;
        public string Name;
        public string FullName;

        public string MDMKey;

        public DateTime? Validity;

        public int? TypeID;
        public string TypeName;
        public bool? NonResident;

        public string Head;
        public string ContactAddress;
        public string INN;
        public string KPP;
        public string OGRN;
        public string OKVED;

        public string IdentityDocument;
        public string IdentityDocumentKind;
        public DateTime? IdentityDocumentIssueDate;
        public string IdentityDocumentSeries;
        public string IdentityDocumentNumber;
        public string IdentityDocumentIssuedBy;
        public Guid? CountryRegistrationID;
        public string CountryRegistrationName;

        public int? SpecialSignID;
        public string SpecialSignName;

        public string Comment;

        public Guid? DirectionID;
        public string DirectionName;

        /// <summary>
        /// Дата одобрения СБ
        /// </summary>
        public DateTime? DateApproval;

        /// <summary>
        /// Состояние
        /// </summary>
        public string StatusName;
    }
}
