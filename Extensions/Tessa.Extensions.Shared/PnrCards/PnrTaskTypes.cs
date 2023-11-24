using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrTaskTypes
    {
        /// <summary>
        /// Task type identifier for "Отправка заявки контрагента": {8B94A5E8-2202-4815-80D3-65CCC821FF10}.
        /// </summary>
        public static readonly Guid PnrPartnerRequestStartIntegrationTypeID = new Guid(0x8b94a5e8, 0x2202, 0x4815, 0x80, 0xd3, 0x65, 0xcc, 0xc8, 0x21, 0xff, 0x10);

        /// <summary>
        /// Task type name for "Отправка заявки контрагента".
        /// </summary>
        public const string PnrPartnerRequestStartIntegrationTypeName = "PnrPartnerRequestStartIntegration";


        /// <summary>
        /// Task type identifier for "Подписание": {6A0004CB-DBA8-4126-ACFA-6A6734E3910D}.
        /// </summary>
        public static readonly Guid PnrTaskSigningTypeID = new Guid(0x6a0004cb, 0xdba8, 0x4126, 0xac, 0xfa, 0x6a, 0x67, 0x34, 0xe3, 0x91, 0x0d);

        /// <summary>
        /// Task type name for "Подписание".
        /// </summary>
        public const string PnrTaskSigningTypeName = "PnrTaskSigning";


        /// <summary>
        /// Task type identifier for "Подписание": {0E141BAC-01D3-4178-96CB-6A6FCCCABB3F}.
        /// </summary>
        public static readonly Guid PnrTaskSignedTypeID = new Guid(0x0e141bac, 0x01d3, 0x4178, 0x96, 0xcb, 0x6a, 0x6f, 0xcc, 0xca, 0xbb, 0x3f);

        /// <summary>
        /// Task type name for "Подписание".
        /// </summary>
        public const string PnrTaskSignedTypeName = "PnrTaskSigned";

        /// <summary>
        /// Task type identifier for "Регистрация": {57859268-F6B2-4FF2-8B24-E2F76DDF878C}.
        /// </summary>
        public static readonly Guid PnrRegistrationTypeID = new Guid(0x57859268, 0xf6b2, 0x4ff2, 0x8b, 0x24, 0xe2, 0xf7, 0x6d, 0xdf, 0x87, 0x8c);

        /// <summary>
        /// Task type name for "Регистрация".
        /// </summary>
        public const string PnrRegistrationTypeName = "PnrRegistration";


        /// <summary>
        /// Task type identifier for "PnrRegistrationAutoStart": {F5BDEAB5-8B4A-4363-BCB3-3B541A09566D}.
        /// </summary>
        public static readonly Guid PnrRegistrationAutoStartTypeID = new Guid(0xf5bdeab5, 0x8b4a, 0x4363, 0xbc, 0xb3, 0x3b, 0x54, 0x1a, 0x09, 0x56, 0x6d);

        /// <summary>
        /// Task type name for "PnrRegistrationAutoStart".
        /// </summary>
        public const string PnrRegistrationAutoStartTypeName = "PnrRegistrationAutoStart";


    }
}
