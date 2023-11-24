using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Tessa.Extensions.Server.Integration.Models
{
    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    [System.Xml.Serialization.XmlRootAttribute("Message", Namespace = "http://esb.axelot.ru", IsNullable = false)]
    public partial class PnrReqOnPartnerRequest
    {

        private PnrReqOnPartnerRequestBody bodyField;

        private ushort classIdField;

        private string creationTimeField;

        private string idField;

        private bool needAcknowledgmentField;

        private string propertiesField;

        private string receiversField;

        private string replyToField;

        private string sourceField;

        private string typeField;

        private string correlationIdField;

        public PnrReqOnPartnerRequestBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        /// <remarks/>
        public ushort ClassId
        {
            get
            {
                return this.classIdField;
            }
            set
            {
                this.classIdField = value;
            }
        }

        /// <remarks/>
        public string CreationTime
        {
            get
            {
                return this.creationTimeField;
            }
            set
            {
                this.creationTimeField = value;
            }
        }

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public bool NeedAcknowledgment
        {
            get
            {
                return this.needAcknowledgmentField;
            }
            set
            {
                this.needAcknowledgmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        public string Receivers
        {
            get
            {
                return this.receiversField;
            }
            set
            {
                this.receiversField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ReplyTo
        {
            get
            {
                return this.replyToField;
            }
            set
            {
                this.replyToField = value;
            }
        }

        /// <remarks/>
        public string Source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string CorrelationId
        {
            get
            {
                return this.correlationIdField;
            }
            set
            {
                this.correlationIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrReqOnPartnerRequestBody
    {

        private PnrReqOnPartnerRequestBodyContractRequestData contractRequestDataField;

        /// <remarks/>
        public PnrReqOnPartnerRequestBodyContractRequestData ContractRequestData
        {
            get
            {
                return this.contractRequestDataField;
            }
            set
            {
                this.contractRequestDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrReqOnPartnerRequestBodyContractRequestData
    {

        private string mDM_KeyField;

        private bool пометкаУдаленияField;

        private string идентификаторЗаписиПодписчикаField;

        private string идентификаторЗаявкиField;

        private string системаИсточникField;

        private string идентификаторЗаписиField;

        private string наименованиеField;

        private string кодField;

        private PnrReqOnPartnerRequestBodyContractRequestDataЗаявитель заявительField;

        private string документУдостоверяющийЛичностьField;

        private string иННField;

        private string кодПоОКПОField;

        private string комментарийField;

        private string кППField;

        private object нерезидентField;

        private string оГРНField;

        private string полноеНаименованиеField;

        private string юрФизЛицоField;

        private string странаРегистрацииField;

        private object серияНомерСвидетельстваField;

        private object датаВыдачиСвидетельстваField;

        private object датаРожденияField;

        private object видДокументаField;

        private object серияField;

        private object номерField;

        private object кемВыданField;

        private object когдаВыданField;

        private object содержаниеЗаданияField;

        private object обособленноеПодразделениеField;

        private object юридическийАдресField;

        private object фактическийАдресField;

        private object телефонField;

        private object emailField;

        private object почтовыйАдресField;

        private string п_ИдентификаторЗаявкиСЭДField;

        private string п_СтатусField;

        private DateTime? п_ДатаУстановкиСтатусаField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string MDM_Key
        {
            get
            {
                return this.mDM_KeyField;
            }
            set
            {
                this.mDM_KeyField = value;
            }
        }

        /// <remarks/>
        public bool ПометкаУдаления
        {
            get
            {
                return this.пометкаУдаленияField;
            }
            set
            {
                this.пометкаУдаленияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ИдентификаторЗаписиПодписчика
        {
            get
            {
                return this.идентификаторЗаписиПодписчикаField;
            }
            set
            {
                this.идентификаторЗаписиПодписчикаField = value;
            }
        }

        /// <remarks/>
        public string ИдентификаторЗаявки
        {
            get
            {
                return this.идентификаторЗаявкиField;
            }
            set
            {
                this.идентификаторЗаявкиField = value;
            }
        }

        /// <remarks/>
        public string СистемаИсточник
        {
            get
            {
                return this.системаИсточникField;
            }
            set
            {
                this.системаИсточникField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ИдентификаторЗаписи
        {
            get
            {
                return this.идентификаторЗаписиField;
            }
            set
            {
                this.идентификаторЗаписиField = value;
            }
        }

        /// <remarks/>
        public string Наименование
        {
            get
            {
                return this.наименованиеField;
            }
            set
            {
                this.наименованиеField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Код
        {
            get
            {
                return this.кодField;
            }
            set
            {
                this.кодField = value;
            }
        }

        /// <remarks/>
        public PnrReqOnPartnerRequestBodyContractRequestDataЗаявитель Заявитель
        {
            get
            {
                return this.заявительField;
            }
            set
            {
                this.заявительField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ДокументУдостоверяющийЛичность
        {
            get
            {
                return this.документУдостоверяющийЛичностьField;
            }
            set
            {
                this.документУдостоверяющийЛичностьField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(IsNullable = true)]
        public string ИНН
        {
            get
            {
                return this.иННField;
            }
            set
            {
                this.иННField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string КодПоОКПО
        {
            get
            {
                return this.кодПоОКПОField;
            }
            set
            {
                this.кодПоОКПОField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Комментарий
        {
            get
            {
                return this.комментарийField;
            }
            set
            {
                this.комментарийField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(IsNullable = true)]
        public string КПП
        {
            get
            {
                return this.кППField;
            }
            set
            {
                this.кППField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Нерезидент
        {
            get
            {
                return this.нерезидентField;
            }
            set
            {
                this.нерезидентField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ОГРН
        {
            get
            {
                return this.оГРНField;
            }
            set
            {
                this.оГРНField = value;
            }
        }

        /// <remarks/>
        public string ПолноеНаименование
        {
            get
            {
                return this.полноеНаименованиеField;
            }
            set
            {
                this.полноеНаименованиеField = value;
            }
        }

        /// <remarks/>
        public string ЮрФизЛицо
        {
            get
            {
                return this.юрФизЛицоField;
            }
            set
            {
                this.юрФизЛицоField = value;
            }
        }

        /// <remarks/>
        public string СтранаРегистрации
        {
            get
            {
                return this.странаРегистрацииField;
            }
            set
            {
                this.странаРегистрацииField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object СерияНомерСвидетельства
        {
            get
            {
                return this.серияНомерСвидетельстваField;
            }
            set
            {
                this.серияНомерСвидетельстваField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ДатаВыдачиСвидетельства
        {
            get
            {
                return this.датаВыдачиСвидетельстваField;
            }
            set
            {
                this.датаВыдачиСвидетельстваField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ДатаРождения
        {
            get
            {
                return this.датаРожденияField;
            }
            set
            {
                this.датаРожденияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ВидДокумента
        {
            get
            {
                return this.видДокументаField;
            }
            set
            {
                this.видДокументаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Серия
        {
            get
            {
                return this.серияField;
            }
            set
            {
                this.серияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Номер
        {
            get
            {
                return this.номерField;
            }
            set
            {
                this.номерField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object КемВыдан
        {
            get
            {
                return this.кемВыданField;
            }
            set
            {
                this.кемВыданField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object КогдаВыдан
        {
            get
            {
                return this.когдаВыданField;
            }
            set
            {
                this.когдаВыданField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object СодержаниеЗадания
        {
            get
            {
                return this.содержаниеЗаданияField;
            }
            set
            {
                this.содержаниеЗаданияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ОбособленноеПодразделение
        {
            get
            {
                return this.обособленноеПодразделениеField;
            }
            set
            {
                this.обособленноеПодразделениеField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ЮридическийАдрес
        {
            get
            {
                return this.юридическийАдресField;
            }
            set
            {
                this.юридическийАдресField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ФактическийАдрес
        {
            get
            {
                return this.фактическийАдресField;
            }
            set
            {
                this.фактическийАдресField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Телефон
        {
            get
            {
                return this.телефонField;
            }
            set
            {
                this.телефонField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ПочтовыйАдрес
        {
            get
            {
                return this.почтовыйАдресField;
            }
            set
            {
                this.почтовыйАдресField = value;
            }
        }

        /// <remarks/>
        public string п_ИдентификаторЗаявкиСЭД
        {
            get
            {
                return this.п_ИдентификаторЗаявкиСЭДField;
            }
            set
            {
                this.п_ИдентификаторЗаявкиСЭДField = value;
            }
        }

        /// <remarks/>
        public string п_Статус
        {
            get
            {
                return this.п_СтатусField;
            }
            set
            {
                this.п_СтатусField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.DateTime? п_ДатаУстановкиСтатуса
        {
            get
            {
                return this.п_ДатаУстановкиСтатусаField;
            }
            set
            {
                this.п_ДатаУстановкиСтатусаField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrReqOnPartnerRequestBodyContractRequestDataЗаявитель
    {

        private string полноеИмяField;

        private string доменноеИмяField;

        private string emailField;

        /// <remarks/>
        public string ПолноеИмя
        {
            get
            {
                return this.полноеИмяField;
            }
            set
            {
                this.полноеИмяField = value;
            }
        }

        /// <remarks/>
        public string ДоменноеИмя
        {
            get
            {
                return this.доменноеИмяField;
            }
            set
            {
                this.доменноеИмяField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
    }
}
