using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Web.Models
{
    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    [System.Xml.Serialization.XmlRootAttribute("Message", Namespace = "http://esb.axelot.ru", IsNullable = false)]
    public partial class PnrServicePartnerRequest
    {
        private PnrPartnerRequestBody bodyField;

        private ushort classIdField;

        private System.DateTime creationTimeField;

        private string idField;

        private string sourceField;

        private string typeField;

        /// <remarks/>
        public PnrPartnerRequestBody Body
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
        public System.DateTime CreationTime
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrPartnerRequestBody
    {

        private PnrPartnerRequestBodyClassData classDataField;

        /// <remarks/>
        public PnrPartnerRequestBodyClassData classData
        {
            get
            {
                return this.classDataField;
            }
            set
            {
                this.classDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrPartnerRequestBodyClassData
    {

        private string mDM_KeyField;

        private string идентификаторЗаявкиСЭД_KeyField;

        private string кодField;

        private string наименованиеField;

        private string полноеНаименованиеField;

        private string юрФизЛицоField;

        private string иННField;

        private string кППField;

        private string кодПоОКПОField;

        private string оГРНField;

        private bool нерезидентField;

        private string аксСтранаРегистрацииField;

        private string аксСерияНомерСвидетельстваField;

        private string аксДатаВыдачиСвидетельстваField;

        private string аксДатаРожденияField;

        private string документУдостоверяющийЛичностьField;

        private string комментарийField;

        private string аксВидДокументаField;

        private string серияField;

        private string номерField;

        private string кемВыданField;

        private string когдаВыданField;

        private bool пометкаУдаленияField;

        private string эталоннаяПозицияField;

        private PnrPartnerRequestBodyClassDataRow[] контактнаяИнформацияField;

        /// <remarks/>
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

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ИдентификаторЗаявкиСЭД
        {
            get
            {
                return this.идентификаторЗаявкиСЭД_KeyField;
            }
            set
            {
                this.идентификаторЗаявкиСЭД_KeyField = value;
            }
        }

        /// <remarks/>
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
        public bool Нерезидент
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
        public string аксСтранаРегистрации
        {
            get
            {
                return this.аксСтранаРегистрацииField;
            }
            set
            {
                this.аксСтранаРегистрацииField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксСерияНомерСвидетельства
        {
            get
            {
                return this.аксСерияНомерСвидетельстваField;
            }
            set
            {
                this.аксСерияНомерСвидетельстваField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксДатаВыдачиСвидетельства
        {
            get
            {
                return this.аксДатаВыдачиСвидетельстваField;
            }
            set
            {
                this.аксДатаВыдачиСвидетельстваField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксДатаРождения
        {
            get
            {
                return this.аксДатаРожденияField;
            }
            set
            {
                this.аксДатаРожденияField = value;
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксВидДокумента
        {
            get
            {
                return this.аксВидДокументаField;
            }
            set
            {
                this.аксВидДокументаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Серия
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
        public string Номер
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
        public string КемВыдан
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
        public string КогдаВыдан
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
        public string ЭталоннаяПозиция
        {
            get
            {
                return this.эталоннаяПозицияField;
            }
            set
            {
                this.эталоннаяПозицияField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public PnrPartnerRequestBodyClassDataRow[] КонтактнаяИнформация
        {
            get
            {
                return this.контактнаяИнформацияField;
            }
            set
            {
                this.контактнаяИнформацияField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrPartnerRequestBodyClassDataRow
    {

        private string типField;

        private string видField;

        private string представлениеField;

        private PnrPartnerRequestBodyClassDataRowЗначенияПолей значенияПолейField;

        private string странаField;

        private string регионField;

        private string городField;

        private string адресЭПField;

        private string доменноеИмяСервераField;

        private string номерТелефонаField;

        private string номерТелефонаБезКодовField;

        /// <remarks/>
        public string Тип
        {
            get
            {
                return this.типField;
            }
            set
            {
                this.типField = value;
            }
        }

        /// <remarks/>
        public string Вид
        {
            get
            {
                return this.видField;
            }
            set
            {
                this.видField = value;
            }
        }

        /// <remarks/>
        public string Представление
        {
            get
            {
                return this.представлениеField;
            }
            set
            {
                this.представлениеField = value;
            }
        }

        /// <remarks/>
        public PnrPartnerRequestBodyClassDataRowЗначенияПолей ЗначенияПолей
        {
            get
            {
                return this.значенияПолейField;
            }
            set
            {
                this.значенияПолейField = value;
            }
        }

        /// <remarks/>
        public string Страна
        {
            get
            {
                return this.странаField;
            }
            set
            {
                this.странаField = value;
            }
        }

        /// <remarks/>
        public string Регион
        {
            get
            {
                return this.регионField;
            }
            set
            {
                this.регионField = value;
            }
        }

        /// <remarks/>
        public string Город
        {
            get
            {
                return this.городField;
            }
            set
            {
                this.городField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string АдресЭП
        {
            get
            {
                return this.адресЭПField;
            }
            set
            {
                this.адресЭПField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ДоменноеИмяСервера
        {
            get
            {
                return this.доменноеИмяСервераField;
            }
            set
            {
                this.доменноеИмяСервераField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string НомерТелефона
        {
            get
            {
                return this.номерТелефонаField;
            }
            set
            {
                this.номерТелефонаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string НомерТелефонаБезКодов
        {
            get
            {
                return this.номерТелефонаБезКодовField;
            }
            set
            {
                this.номерТелефонаБезКодовField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    public partial class PnrPartnerRequestBodyClassDataRowЗначенияПолей
    {

        private КонтактнаяИнформация контактнаяИнформацияField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
        public КонтактнаяИнформация КонтактнаяИнформация
        {
            get
            {
                return this.контактнаяИнформацияField;
            }
            set
            {
                this.контактнаяИнформацияField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.v8.1c.ru/ssl/contactinfo", IsNullable = false)]
    public partial class КонтактнаяИнформация
    {

        private string комментарийField;

        private КонтактнаяИнформацияСостав составField;

        private string представлениеField;

        /// <remarks/>
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
        public КонтактнаяИнформацияСостав Состав
        {
            get
            {
                return this.составField;
            }
            set
            {
                this.составField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Представление
        {
            get
            {
                return this.представлениеField;
            }
            set
            {
                this.представлениеField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "Адрес", Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class КонтактнаяИнформацияСостав
    {

        private КонтактнаяИнформацияСоставСостав составField;

        private string странаField;

        /// <remarks/>
        public КонтактнаяИнформацияСоставСостав Состав
        {
            get
            {
                return this.составField;
            }
            set
            {
                this.составField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Страна
        {
            get
            {
                return this.странаField;
            }
            set
            {
                this.странаField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "АдресРФ", Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class КонтактнаяИнформацияСоставСостав
    {

        private string субъектРФField;

        private string городField;

        private string улицаField;

        private КонтактнаяИнформацияСоставСоставДопАдрЭл[] допАдрЭлField;

        /// <remarks/>
        public string СубъектРФ
        {
            get
            {
                return this.субъектРФField;
            }
            set
            {
                this.субъектРФField = value;
            }
        }

        /// <remarks/>
        public string Город
        {
            get
            {
                return this.городField;
            }
            set
            {
                this.городField = value;
            }
        }

        /// <remarks/>
        public string Улица
        {
            get
            {
                return this.улицаField;
            }
            set
            {
                this.улицаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ДопАдрЭл")]
        public КонтактнаяИнформацияСоставСоставДопАдрЭл[] ДопАдрЭл
        {
            get
            {
                return this.допАдрЭлField;
            }
            set
            {
                this.допАдрЭлField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class КонтактнаяИнформацияСоставСоставДопАдрЭл
    {

        private КонтактнаяИнформацияСоставСоставДопАдрЭлНомер номерField;

        private string типАдрЭлField;

        private bool типАдрЭлFieldSpecified;

        private string значениеField;

        private bool значениеFieldSpecified;

        /// <remarks/>
        public КонтактнаяИнформацияСоставСоставДопАдрЭлНомер Номер
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ТипАдрЭл
        {
            get
            {
                return this.типАдрЭлField;
            }
            set
            {
                this.типАдрЭлField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ТипАдрЭлSpecified
        {
            get
            {
                return this.типАдрЭлFieldSpecified;
            }
            set
            {
                this.типАдрЭлFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Значение
        {
            get
            {
                return this.значениеField;
            }
            set
            {
                this.значениеField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ЗначениеSpecified
        {
            get
            {
                return this.значениеFieldSpecified;
            }
            set
            {
                this.значениеFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class КонтактнаяИнформацияСоставСоставДопАдрЭлНомер
    {

        private string типField;

        private string значениеField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Тип
        {
            get
            {
                return this.типField;
            }
            set
            {
                this.типField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Значение
        {
            get
            {
                return this.значениеField;
            }
            set
            {
                this.значениеField = value;
            }
        }
    }
}

