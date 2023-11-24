using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    public class PnrContract
    {
        public Guid ID;
        public string Subject;

        /// <summary>
        /// Контрагент
        /// </summary>
        public Guid? PartnerID;

        /// <summary>
        /// Организация ГК Пионер
        /// </summary>
        public Guid? OrganizationID;

        /// <summary>
        /// Проект
        /// </summary>
        public Guid? ProjectID;

        /// <summary>
        /// ЦФО
        /// </summary>
        public Guid? CFOID;

        /// <summary>
        /// Наименование статьи затрат
        /// </summary>
        public Guid? CostItemID;

        /// <summary>
        /// Сумма договора
        /// </summary>
        public decimal? Amount;

        /// <summary>
        /// Планируемая дата актирования
        /// </summary>
        public DateTime? PlannedActDate;

        /// <summary>
        /// Форма договора
        /// </summary>
        public Guid? FormID;

        /// <summary>
        /// Тип договора
        /// </summary>
        public Guid? TypeID;

        /// <summary>
        /// Подписант
        /// </summary>
        public Guid? SignatoryID;
    }
}
