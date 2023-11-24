using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrVatRateHelper
    {
        /// <summary>
        /// Значение ставки НДС по ID из справочника "Ставки НДС"
        /// </summary>
        public static int? GetVatRateValueByID(int? id)
        {
            if (id == null)
            {
                return null;
            }
            switch (id.Value)
            {
                // Без НДС
                case 0:
                    return 0;
                // 0%
                case 1:
                    return 0;
                // 10%
                case 2:
                    return 10;
                // 18%
                case 3:
                    return 18;
                // 20%
                case 4:
                    return 20;
            }
            return null;
        }
    }
}
