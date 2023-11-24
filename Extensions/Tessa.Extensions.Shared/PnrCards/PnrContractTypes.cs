using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrContractTypes
    {
        /// <summary>
        /// Договор поставки
        /// </summary>
        public static readonly Guid PnrDeliveryContractID = new Guid("7f9b28ec-1a6f-4359-88bf-957f367fadd9");
        public static readonly string PnrDeliveryContractName = "Договор поставки";

        /// <summary>
        /// Отделка
        /// </summary>
        public static readonly Guid PnrFinishingID = new Guid("e8d7dfc0-5ae1-46bf-a101-539774c53277");
        public static readonly string PnrFinishingName = "Отделка";

        /// <summary>
        /// Благоустройство
        /// </summary>
        public static readonly Guid PnrLandscapingID = new Guid("9abfd947-9e02-4f3f-a1c2-022ed7c4d381");
        public static readonly string PnrLandscapingName = "Благоустройство";

        /// <summary>
        /// Инженерное обеспечение строительства
        /// </summary>
        public static readonly Guid PnrConstructionEngineeringID = new Guid("8dcca407-3d23-4a69-bd88-3d3ddf8b5550");
        public static readonly string PnrConstructionEngineeringName = "Инженерное обеспечение строительства";

        /// <summary>
        /// Договор генподряда
        /// </summary>
        public static readonly Guid PnrGeneralContractID = new Guid("eabace15-48ce-4dd2-a783-a2eea50ea976");
        public static readonly string PnrGeneralContractName = "Договор генподряда";

        /// <summary>
        /// Договор ген.проектирования
        /// </summary>
        public static readonly Guid PnrGeneralDesignAgreementID = new Guid("3353130a-7ff8-4e15-8e85-55bdb96f3f80");
        public static readonly string PnrGeneralDesignAgreementName = "Договор ген.проектирования";

        /// <summary>
        /// Строительные работы
        /// </summary>
        public static readonly Guid PnrConstructionWorksID = new Guid("d27d1cfb-2152-4b64-8606-c075f62d3284");
        public static readonly string PnrConstructionWorksName = "Строительные работы";
    }
}
