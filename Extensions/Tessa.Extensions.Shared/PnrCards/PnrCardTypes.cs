using System;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrCardTypes
    {
        public static readonly Guid PnrIncomingTypeID = new Guid("476fa752-133d-4571-8f28-86002241f2fe");
        public const string PnrIncomingTypeName = "PnrIncoming";

        public static readonly Guid PnrOutgoingTypeID = new Guid("40dab24a-0b6f-4609-947c-f1916348a540");
        public const string PnrOutgoingTypeName = "PnrOutgoing";

        public static readonly Guid PnrPowerAttorneyTypeID = new Guid("f9c07ae1-4e87-4cfe-8229-26ce6af5c326");
        public const string PnrPowerAttorneyTypeName = "PnrPowerAttorney";

        public static readonly Guid PnrRegulationTypeID = new Guid("cd45788f-1576-4836-83c1-c55714eba28a");
        public const string PnrRegulationTypeName = "PnrRegulation";

        public static readonly Guid PnrContractTypeID = new Guid("1c7a5718-09ae-4f65-aa67-e66f23bb7aee");
        public const string PnrContractTypeName = "PnrContract";

        public static readonly Guid PnrSupplementaryAgreementTypeID = new Guid("f5a33228-32ae-483f-beca-8b2e3453a615");
        public const string PnrSupplementaryAgreementTypeName = "PnrSupplementaryAgreement";

        public static readonly Guid PnrOrderTypeID = new Guid("df141f0f-7e73-48fb-9cdb-6d46665cc0fb");
        public const string PnrOrderTypeName = "PnrOrder";

        public static readonly Guid PnrServiceNoteTypeID = new Guid("dceb0c7e-4147-410c-8a6c-f30781226007");
        public const string PnrServiceNoteTypeName = "PnrServiceNote";

        public static readonly Guid PnrPartnerRequestTypeID = new Guid("ca76dbb5-e4f0-46b7-b5fa-2f7f77c7cae2");
        public const string PnrPartnerRequestTypeName = "PnrPartnerRequest";

        public static readonly Guid PnrErrandTypeID = new Guid("531e41ec-639f-41a9-9313-94f3eada0427");
        public const string PnrErrandTypeName = "PnrErrand";

        public static readonly Guid PnrActTypeID = new Guid("156df436-74e3-4e08-aba8-cbc609c6c1c7");
        public const string PnrActTypeName = "PnrAct";

        public static readonly Guid PnrTenderTypeID = new Guid("78cc3cc5-6314-45c1-bafc-5d41d7da7640");
        public const string PnrTenderTypeName = "PnrTender";

        public static readonly Guid PnrTemplateTypeID = new Guid("dc10a79d-4bb2-4aad-acb8-82d5838408a9");
        public const string PnrTemplateTypeName = "PnrTemplate";

        public static readonly Guid PnrIncomingUKTypeID = new Guid("42eb6143-d431-4bb9-b4bf-19a521205ca5");
        public const string PnrIncomingUKTypeName = "PnrIncomingUK";

        public static readonly Guid PnrOutgoingUKTypeID = new Guid("10e5967d-8282-4b43-89c6-8d8c9fd9558f");
        public const string PnrOutgoingUKTypeName = "PnrOutgoingUK";

        public static readonly Guid PnrOrderUKTypeID = new Guid("8d8d1098-3b12-4a77-a988-4278f11d9039");
        public const string PnrOrderUKTypeName = "PnrOrderUK";

        public static readonly Guid PnrContractUKTypeID = new Guid("25ea1e75-6ff9-4fd1-94e3-f6bc266d6544");
        public const string PnrContractUKTypeName = "PnrContractUK";

        public static readonly Guid PnrSupplementaryAgreementUKTypeID = new Guid("87adb0cb-7c5f-4c82-974f-5d4e3c4a050f");
        public const string PnrSupplementaryAgreementUKTypeName = "PnrSupplementaryAgreementUK";

        public static readonly Guid PnrPartnerTypeID = new Guid("b9a1f125-ab1d-4cff-929f-5ad8351bda4f");
        public const string PnrPartnerTypeName = "Partner";

        /// <summary>
        /// Card type identifier for "PnrCFO": {38BBD7ED-AB6F-4A12-81C2-EA0069DA316F}.
        /// </summary>
        public static readonly Guid PnrCFOTypeID = new Guid(0x38bbd7ed, 0xab6f, 0x4a12, 0x81, 0xc2, 0xea, 0x00, 0x69, 0xda, 0x31, 0x6f);

        /// <summary>
        /// Card type name for "PnrCFO".
        /// </summary>
        public const string PnrCFOTypeName = "PnrCFO";

        /// <summary>
        /// Card type identifier for "PnrOrganization": {A668F7EA-EFCD-47F0-A3C7-C4D1E7ED0BC8}.
        /// </summary>
        public static readonly Guid PnrOrganizationTypeID = new Guid(0xa668f7ea, 0xefcd, 0x47f0, 0xa3, 0xc7, 0xc4, 0xd1, 0xe7, 0xed, 0x0b, 0xc8);

        /// <summary>
        /// Card type name for "PnrOrganization".
        /// </summary>
        public const string PnrOrganizationTypeName = "PnrOrganization";

        /// <summary>
        /// Card type identifier for "PnrProject": {C17A5031-E7D8-4EA6-B03F-4C88B4BD6063}.
        /// </summary>
        public static readonly Guid PnrProjectTypeID = new Guid(0xc17a5031, 0xe7d8, 0x4ea6, 0xb0, 0x3f, 0x4c, 0x88, 0xb4, 0xbd, 0x60, 0x63);

        /// <summary>
        /// Card type name for "PnrProject".
        /// </summary>
        public const string PnrProjectTypeName = "PnrProject";

        /// <summary>
        /// Card type identifier for "PnrCostItem": {FD2109F5-BB9B-4911-BEC8-2C93A77C0523}.
        /// </summary>
        public static readonly Guid PnrCostItemTypeID = new Guid(0xfd2109f5, 0xbb9b, 0x4911, 0xbe, 0xc8, 0x2c, 0x93, 0xa7, 0x7c, 0x05, 0x23);

        /// <summary>
        /// Card type name for "PnrCostItem".
        /// </summary>
        public const string PnrCostItemTypeName = "PnrCostItem";
    }
}
