using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrContractKinds
    {
        // Виды договора
        public static readonly Guid PnrContractWithBuyersID = new Guid("7EDE7958-E642-490C-B458-32C034CCB9D6");
        public static readonly string PnrContractWithBuyersName = "С покупателями";

        public static readonly Guid PnrContractCFOID = new Guid("252A93B2-9FE5-4EE7-B284-6268714DB5EE");
        public static readonly string PnrContractCFOName = "ЦФО";

        public static readonly Guid PnrContractDUPID = new Guid("2B35C1F5-EBAF-4F70-B030-6DCEBF6CE550");
        public static readonly string PnrContractDUPName = "ДУП";

        public static readonly Guid PnrContractIntercompanyID = new Guid("5D232548-C3FA-414E-94FA-9CCFD187DD4E");
        public static readonly string PnrContractIntercompanyName = "Внутрихолдинговый";

        public static readonly Guid PnrContractUKID = new Guid("3A7F78FF-8138-4C0D-9437-00DD16328B67");
        public static readonly string PnrContractUKName = "УК ПС";

        // Виды договора ДУП
        public static readonly Guid PnrContractDUPBuildingID = new Guid("F280916C-81D1-46D9-982F-6B8EB7E6196F");
        public static readonly string PnrContractDUPBuildingName = "Строительные";

        public static readonly Guid PnrContractDUPNotBuildingID = new Guid("3E4408CA-C37B-40C9-9067-99AC80CB32CE");
        public static readonly string PnrContractDUPNotBuildingName = "Нестроительные";

        public static readonly Guid PnrContractDUPIntragroupID = new Guid("22F59E52-BD8E-43E8-A2A2-9481FD394696");
        public static readonly string PnrContractDUPIntragroupName = "Внутригрупповой";

        // Виды договора 1С
        public static readonly Guid PnrContract1CWithSupplierID = new Guid("8A045F3A-448A-4AF1-A6C5-50DDBBE559DE");
        public static readonly string PnrContract1CWithSupplierName = "С поставщиком";
    }
}
