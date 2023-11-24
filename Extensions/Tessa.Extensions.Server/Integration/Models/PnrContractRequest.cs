using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Integration.Models
{
    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    [System.Xml.Serialization.XmlRootAttribute("Message", Namespace = "http://esb.axelot.ru", IsNullable = false)]
    public partial class PnrContractRequest
    {

        private PnrContractRequestBody bodyField;

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

        /// <remarks/>
        public PnrContractRequestBody Body
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
    public partial class PnrContractRequestBody
    {

        private PnrContractRequestBodyContractData contractDataField;

        /// <remarks/>
        public PnrContractRequestBodyContractData ContractData
        {
            get
            {
                return this.contractDataField;
            }
            set
            {
                this.contractDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrContractRequestBodyContractData
    {

        private string mDM_KeyField;

        private bool пометкаУдаленияField;

        private string идентификаторЗаявкиField;

        private string системаИсточникField;

        private string идентификаторЗаписиField;

        private string наименованиеField;

        private string кодField;

        private string валютаРасчетовField;

        private System.DateTime датаДоговораField;

        private System.DateTime срокДоговораField;

        private System.DateTime срокДействияField;

        private string контрагентField;

        private string номерДоговораField;

        private string организацияField;

        private string основнаяСтатьяОборотовField;

        private string цФОField;

        private string суммаДоговораField;

        private string проектField;

        private string идентификаторРодительскойЗаявкиСЭДField;

        private bool вГОField;

        private bool направлениеЖКХField;

        private string видДоговораField;

        private string видАгентскогоДоговораField;

        private string видВзаиморасчетовField;

        private string ссылкаНаКаталогФайлаField;

        private bool оплатаВВалютеField;

        private string комментарийField;

        private PnrContractRequestBodyContractDataЗаявитель заявительField;

        private object содержаниеЗаданияField;

        private object п_ПомещениеField;

        private object п_ПроцентField;

        private object п_ПроцентыСДатыПолученияЗаймаField;

        private object п_СуммаПервичногоЗаймаField;

        private object п_ОплатаПоЭскроуField;

        private object аксПроектField;

        private object п_СвязанныйДоговорField;

        private string п_ТекущийСтатусВСЭДField;

        private string п_АвансField;

        private string п_ПроцентАвансаField;

        private string п_СуммаАвансаField;

        private string п_ПроцентГУField;

        private object п_ГарантийныйСрокField;

        private string п_СтавкаНДСField;

        private object п_ВидДоговораДУПField;

        private DateTime? п_ДатаОкончанияField;

        private object п_ОтсрочкаРДField;

        private object п_СтатусField;

        private bool? п_ВБюджетеField;

        private bool предусмотреноПоэтапноеВыполнениеField;

        private string п_ТипДоговораField;

        private string п_ПричинаЗаключенияДСField;

        private System.DateTime? п_СЭД_ДатаАктированияField;

        private string п_ИдентификаторЗаявкиСЭДField;

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
        public string ВалютаРасчетов
        {
            get
            {
                return this.валютаРасчетовField;
            }
            set
            {
                this.валютаРасчетовField = value;
            }
        }

        /// <remarks/>
        public System.DateTime ДатаДоговора
        {
            get
            {
                return this.датаДоговораField;
            }
            set
            {
                this.датаДоговораField = value;
            }
        }

        /// <remarks/>
        public System.DateTime СрокДоговора
        {
            get
            {
                return this.срокДоговораField;
            }
            set
            {
                this.срокДоговораField = value;
            }
        }

        /// <remarks/>
        public System.DateTime СрокДействия
        {
            get
            {
                return this.срокДействияField;
            }
            set
            {
                this.срокДействияField = value;
            }
        }

        /// <remarks/>
        public string Контрагент
        {
            get
            {
                return this.контрагентField;
            }
            set
            {
                this.контрагентField = value;
            }
        }

        /// <remarks/>
        public string НомерДоговора
        {
            get
            {
                return this.номерДоговораField;
            }
            set
            {
                this.номерДоговораField = value;
            }
        }

        /// <remarks/>
        public string Организация
        {
            get
            {
                return this.организацияField;
            }
            set
            {
                this.организацияField = value;
            }
        }

        /// <remarks/>
        public string ОсновнаяСтатьяОборотов
        {
            get
            {
                return this.основнаяСтатьяОборотовField;
            }
            set
            {
                this.основнаяСтатьяОборотовField = value;
            }
        }

        /// <remarks/>
        public string ЦФО
        {
            get
            {
                return this.цФОField;
            }
            set
            {
                this.цФОField = value;
            }
        }

        /// <remarks/>
        public string СуммаДоговора
        {
            get
            {
                return this.суммаДоговораField;
            }
            set
            {
                this.суммаДоговораField = value;
            }
        }

        /// <remarks/>
        public string Проект
        {
            get
            {
                return this.проектField;
            }
            set
            {
                this.проектField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ИдентификаторРодительскойЗаявкиСЭД
        {
            get
            {
                return this.идентификаторРодительскойЗаявкиСЭДField;
            }
            set
            {
                this.идентификаторРодительскойЗаявкиСЭДField = value;
            }
        }

        /// <remarks/>
        public bool ВГО
        {
            get
            {
                return this.вГОField;
            }
            set
            {
                this.вГОField = value;
            }
        }

        /// <remarks/>
        public bool НаправлениеЖКХ
        {
            get
            {
                return this.направлениеЖКХField;
            }
            set
            {
                this.направлениеЖКХField = value;
            }
        }

        /// <remarks/>
        public string ВидДоговора
        {
            get
            {
                return this.видДоговораField;
            }
            set
            {
                this.видДоговораField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ВидАгентскогоДоговора
        {
            get
            {
                return this.видАгентскогоДоговораField;
            }
            set
            {
                this.видАгентскогоДоговораField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ВидВзаиморасчетов
        {
            get
            {
                return this.видВзаиморасчетовField;
            }
            set
            {
                this.видВзаиморасчетовField = value;
            }
        }

        /// <remarks/>
        public string СсылкаНаКаталогФайла
        {
            get
            {
                return this.ссылкаНаКаталогФайлаField;
            }
            set
            {
                this.ссылкаНаКаталогФайлаField = value;
            }
        }

        /// <remarks/>
        public bool ОплатаВВалюте
        {
            get
            {
                return this.оплатаВВалютеField;
            }
            set
            {
                this.оплатаВВалютеField = value;
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
        public PnrContractRequestBodyContractDataЗаявитель Заявитель
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
        public object п_Помещение
        {
            get
            {
                return this.п_ПомещениеField;
            }
            set
            {
                this.п_ПомещениеField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_Процент
        {
            get
            {
                return this.п_ПроцентField;
            }
            set
            {
                this.п_ПроцентField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_ПроцентыСДатыПолученияЗайма
        {
            get
            {
                return this.п_ПроцентыСДатыПолученияЗаймаField;
            }
            set
            {
                this.п_ПроцентыСДатыПолученияЗаймаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_СуммаПервичногоЗайма
        {
            get
            {
                return this.п_СуммаПервичногоЗаймаField;
            }
            set
            {
                this.п_СуммаПервичногоЗаймаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_ОплатаПоЭскроу
        {
            get
            {
                return this.п_ОплатаПоЭскроуField;
            }
            set
            {
                this.п_ОплатаПоЭскроуField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object аксПроект
        {
            get
            {
                return this.аксПроектField;
            }
            set
            {
                this.аксПроектField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_СвязанныйДоговор
        {
            get
            {
                return this.п_СвязанныйДоговорField;
            }
            set
            {
                this.п_СвязанныйДоговорField = value;
            }
        }

        /// <remarks/>
        public string п_ТекущийСтатусВСЭД
        {
            get
            {
                return this.п_ТекущийСтатусВСЭДField;
            }
            set
            {
                this.п_ТекущийСтатусВСЭДField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string п_Аванс
        {
            get
            {
                return this.п_АвансField;
            }
            set
            {
                this.п_АвансField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string п_ПроцентАванса
        {
            get
            {
                return this.п_ПроцентАвансаField;
            }
            set
            {
                this.п_ПроцентАвансаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string п_СуммаАванса
        {
            get
            {
                return this.п_СуммаАвансаField;
            }
            set
            {
                this.п_СуммаАвансаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string п_ПроцентГУ
        {
            get
            {
                return this.п_ПроцентГУField;
            }
            set
            {
                this.п_ПроцентГУField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_ГарантийныйСрок
        {
            get
            {
                return this.п_ГарантийныйСрокField;
            }
            set
            {
                this.п_ГарантийныйСрокField = value;
            }
        }

        /// <remarks/>
        public string п_СтавкаНДС
        {
            get
            {
                return this.п_СтавкаНДСField;
            }
            set
            {
                this.п_СтавкаНДСField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_ВидДоговораДУП
        {
            get
            {
                return this.п_ВидДоговораДУПField;
            }
            set
            {
                this.п_ВидДоговораДУПField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public DateTime? п_ДатаОкончания
        {
            get
            {
                return this.п_ДатаОкончанияField;
            }
            set
            {
                this.п_ДатаОкончанияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_ОтсрочкаРД
        {
            get
            {
                return this.п_ОтсрочкаРДField;
            }
            set
            {
                this.п_ОтсрочкаРДField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object п_Статус
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
        public bool? п_ВБюджете
        {
            get
            {
                return this.п_ВБюджетеField;
            }
            set
            {
                this.п_ВБюджетеField = value;
            }
        }

        /// <remarks/>
        public bool ПредусмотреноПоэтапноеВыполнение
        {
            get
            {
                return this.предусмотреноПоэтапноеВыполнениеField;
            }
            set
            {
                this.предусмотреноПоэтапноеВыполнениеField = value;
            }
        }

        /// <remarks/>
        public string п_ТипДоговора
        {
            get
            {
                return this.п_ТипДоговораField;
            }
            set
            {
                this.п_ТипДоговораField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string п_ПричинаЗаключенияДС
        {
            get
            {
                return this.п_ПричинаЗаключенияДСField;
            }
            set
            {
                this.п_ПричинаЗаключенияДСField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.DateTime? п_СЭД_ДатаАктирования
        {
            get
            {
                return this.п_СЭД_ДатаАктированияField;
            }
            set
            {
                this.п_СЭД_ДатаАктированияField = value;
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrContractRequestBodyContractDataЗаявитель
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
