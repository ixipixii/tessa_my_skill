using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    public class PnrOrganization
    {
        public Guid ID;
        public string Name;

        public string MDMKey;

        /// <summary>
        /// Индекс юридического лица
        /// </summary>
        public string LegalEntityIndex;

        /// <summary>
        /// Банк
        /// </summary>
        public string Bank;

        /// <summary>
        /// ИНН (Банка)
        /// </summary>
        public string INNBank;

        /// <summary>
        /// БИК
        /// </summary>
        public string BIK;

        /// <summary>
        /// Расчетный счет
        /// </summary>
        public string SettlementAccount;

        /// <summary>
        /// Корр. счет №
        /// </summary>
        public string CorrespondentAccount;

        /// <summary>
        /// Должность руководителя ЮЛ
        /// </summary>
        public string PositionHeadLegalEntity;

        /// <summary>
        /// Главный бухгалтер для процессов
        /// </summary>
        public Guid? ChiefAccountantProcessID;

        /// <summary>
        /// Руководитель ЮЛ
        /// </summary>
        public Guid? HeadLegalEntityID;
        public string HeadLegalEntityName;

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address;
    }
}
