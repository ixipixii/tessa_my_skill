using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrImplementationStages
    {
        /// <summary>
        /// Благоустройство
        /// </summary>
        public static readonly Guid PnrLandscapingID = new Guid("8cdd779f-36db-45b0-b705-0b4e35ebf8f5");
        public static readonly string PnrLandscapingName = "Благоустройство";

        /// <summary>
        /// Отделка
        /// </summary>
        public static readonly Guid PnrFinishingID = new Guid("4738aecd-5633-4e5c-ac95-4a059ffa6500");
        public static readonly string PnrFinishingName = "Отделка";

        /// <summary>
        /// Концепция
        /// </summary>
        public static readonly Guid PnrConceptID = new Guid("4a9db266-634d-4eb8-8ab2-716f2042881c");
        public static readonly string PnrConceptName = "Концепция";

        /// <summary>
        /// ГП
        /// </summary>
        public static readonly Guid PnrGPID = new Guid("49156beb-c9cb-45f0-9cf5-43456f461d8e");
        public static readonly string PnrGPName = "ГП";

        /// <summary>
        /// Инженерия
        /// </summary>
        public static readonly Guid PnrEngineeringID = new Guid("e39d4d35-55e4-42c3-b34a-cb0fefdc7e11");
        public static readonly string PnrEngineeringName = "Инженерия";

        /// <summary>
        /// ПД
        /// </summary>
        public static readonly Guid PnrPDID = new Guid("18974ef6-3caa-4167-9492-79137160247a");
        public static readonly string PnrPDName = "ПД";

        /// <summary>
        /// Прочее
        /// </summary>
        public static readonly Guid PnrOtherID = new Guid("88628106-a5bf-4068-95a6-048be7d9151a");
        public static readonly string PnrOtherName = "Прочее";
    }
}
