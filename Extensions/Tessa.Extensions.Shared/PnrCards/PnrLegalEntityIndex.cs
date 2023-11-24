using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrLegalEntityIndex
    {
        /// <summary>
		/// Индекс ЮЛ при выборе списка организаций: 100 - Вся организация
		/// </summary>
		public static readonly Guid WholeOrganizationID = new Guid("0D2C2DCC-B4DE-4361-8083-A0315B239C09");
        public static readonly string WholeOrganizationName = "Вся организация";
        public static readonly string WholeOrganizationIdx = "100";
    }
}
