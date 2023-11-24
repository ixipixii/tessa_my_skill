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
    public partial class PnrOrganizationRequest
    {

        private PnrOrganizationRequestBody bodyField;

        private ushort classIdField;

        private System.DateTime creationTimeField;

        private string idField;

        private string sourceField;

        private string typeField;

        /// <remarks/>
        public PnrOrganizationRequestBody Body
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
    public partial class PnrOrganizationRequestBody
    {

        private PnrOrganizationRequestBodyClassData classDataField;

        /// <remarks/>
        public PnrOrganizationRequestBodyClassData classData
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
    public partial class PnrOrganizationRequestBodyClassData
    {

        private string mDM_KeyField;

        private string кодField;

        private string наименованиеField;

        private string префиксField;

        private string контрагентField;

        private string полноеНаименованиеField;

        private string юрФизЛицоField;

        private string иННField;

        private string кППField;

        private string кодПоОКПОField;

        private string оГРНField;

        private string аксДатаВыдачиField;

        private DateTime? аксДатаРегиистрацииField;

        private bool аксИностраннаяОрганизацияField;

        private string аксКодНалоговогоОрганаField;

        private string аксКодОКВЭДField;

        private string аксКодОКВЭД2Field;

        private string аксКодОКОПФField;

        private string аксКодОКФСField;

        private string аксКодОрганаПФРField;

        private string аксКодОрганаФСГСField;

        private string аксКодПодчиненностиФССField;

        private string аксНаименованиеИностраннойОрганизацииField;

        private string аксНаименованиеНалоговогоОрганаField;

        private string аксНаименованиеОКВЭДField;

        private string аксНаименованиеОКВЭД2Field;

        private string аксНаименованиеОКОПФField;

        private string аксНаименованиеОКФСField;

        private string аксСокращенноеНаименованиеField;

        private string аксНаименованиеТерриториальногоОрганаПФРField;

        private string аксНаименованиеТерриториальногоОрганаФССField;

        private string аксРегистрационныйНомерПФРField;

        private string аксРегистрационныйНомерФССField;

        private string аксСвидетельствоКодОрганаField;

        private string аксСвидетельствоНаименованиеОрганаField;

        private string аксСерияНомерСвидетельстваField;

        private string аксСтранаПостоянногоМестонахожденияField;

        private string аксСтранаРегистрацииField;

        private bool пометкаУдаленияField;

        private string эталоннаяПозицияField;

        private PnrOrganizationRequestBodyClassDataRow[] контактнаяИнформацияField;

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
        public string Префикс
        {
            get
            {
                return this.префиксField;
            }
            set
            {
                this.префиксField = value;
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксДатаВыдачи
        {
            get
            {
                return this.аксДатаВыдачиField;
            }
            set
            {
                this.аксДатаВыдачиField = value;
            }
        }

        /// <remarks/>
        public DateTime? аксДатаРегиистрации
        {
            get
            {
                return this.аксДатаРегиистрацииField;
            }
            set
            {
                this.аксДатаРегиистрацииField = value;
            }
        }

        /// <remarks/>
        public bool аксИностраннаяОрганизация
        {
            get
            {
                return this.аксИностраннаяОрганизацияField;
            }
            set
            {
                this.аксИностраннаяОрганизацияField = value;
            }
        }

        /// <remarks/>
        public string аксКодНалоговогоОргана
        {
            get
            {
                return this.аксКодНалоговогоОрганаField;
            }
            set
            {
                this.аксКодНалоговогоОрганаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксКодОКВЭД
        {
            get
            {
                return this.аксКодОКВЭДField;
            }
            set
            {
                this.аксКодОКВЭДField = value;
            }
        }

        /// <remarks/>
        public string аксКодОКВЭД2
        {
            get
            {
                return this.аксКодОКВЭД2Field;
            }
            set
            {
                this.аксКодОКВЭД2Field = value;
            }
        }

        /// <remarks/>
        public string аксКодОКОПФ
        {
            get
            {
                return this.аксКодОКОПФField;
            }
            set
            {
                this.аксКодОКОПФField = value;
            }
        }

        /// <remarks/>
        public string аксКодОКФС
        {
            get
            {
                return this.аксКодОКФСField;
            }
            set
            {
                this.аксКодОКФСField = value;
            }
        }

        /// <remarks/>
        public string аксКодОрганаПФР
        {
            get
            {
                return this.аксКодОрганаПФРField;
            }
            set
            {
                this.аксКодОрганаПФРField = value;
            }
        }

        /// <remarks/>
        public string аксКодОрганаФСГС
        {
            get
            {
                return this.аксКодОрганаФСГСField;
            }
            set
            {
                this.аксКодОрганаФСГСField = value;
            }
        }

        /// <remarks/>
        public string аксКодПодчиненностиФСС
        {
            get
            {
                return this.аксКодПодчиненностиФССField;
            }
            set
            {
                this.аксКодПодчиненностиФССField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксНаименованиеИностраннойОрганизации
        {
            get
            {
                return this.аксНаименованиеИностраннойОрганизацииField;
            }
            set
            {
                this.аксНаименованиеИностраннойОрганизацииField = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеНалоговогоОргана
        {
            get
            {
                return this.аксНаименованиеНалоговогоОрганаField;
            }
            set
            {
                this.аксНаименованиеНалоговогоОрганаField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string аксНаименованиеОКВЭД
        {
            get
            {
                return this.аксНаименованиеОКВЭДField;
            }
            set
            {
                this.аксНаименованиеОКВЭДField = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеОКВЭД2
        {
            get
            {
                return this.аксНаименованиеОКВЭД2Field;
            }
            set
            {
                this.аксНаименованиеОКВЭД2Field = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеОКОПФ
        {
            get
            {
                return this.аксНаименованиеОКОПФField;
            }
            set
            {
                this.аксНаименованиеОКОПФField = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеОКФС
        {
            get
            {
                return this.аксНаименованиеОКФСField;
            }
            set
            {
                this.аксНаименованиеОКФСField = value;
            }
        }

        /// <remarks/>
        public string аксСокращенноеНаименование
        {
            get
            {
                return this.аксСокращенноеНаименованиеField;
            }
            set
            {
                this.аксСокращенноеНаименованиеField = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеТерриториальногоОрганаПФР
        {
            get
            {
                return this.аксНаименованиеТерриториальногоОрганаПФРField;
            }
            set
            {
                this.аксНаименованиеТерриториальногоОрганаПФРField = value;
            }
        }

        /// <remarks/>
        public string аксНаименованиеТерриториальногоОрганаФСС
        {
            get
            {
                return this.аксНаименованиеТерриториальногоОрганаФССField;
            }
            set
            {
                this.аксНаименованиеТерриториальногоОрганаФССField = value;
            }
        }

        /// <remarks/>
        public string аксРегистрационныйНомерПФР
        {
            get
            {
                return this.аксРегистрационныйНомерПФРField;
            }
            set
            {
                this.аксРегистрационныйНомерПФРField = value;
            }
        }

        /// <remarks/>
        public string аксРегистрационныйНомерФСС
        {
            get
            {
                return this.аксРегистрационныйНомерФССField;
            }
            set
            {
                this.аксРегистрационныйНомерФССField = value;
            }
        }

        /// <remarks/>
        public string аксСвидетельствоКодОргана
        {
            get
            {
                return this.аксСвидетельствоКодОрганаField;
            }
            set
            {
                this.аксСвидетельствоКодОрганаField = value;
            }
        }

        /// <remarks/>
        public string аксСвидетельствоНаименованиеОргана
        {
            get
            {
                return this.аксСвидетельствоНаименованиеОрганаField;
            }
            set
            {
                this.аксСвидетельствоНаименованиеОрганаField = value;
            }
        }

        /// <remarks/>
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
        public string аксСтранаПостоянногоМестонахождения
        {
            get
            {
                return this.аксСтранаПостоянногоМестонахожденияField;
            }
            set
            {
                this.аксСтранаПостоянногоМестонахожденияField = value;
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
        public PnrOrganizationRequestBodyClassDataRow[] КонтактнаяИнформация
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
    public partial class PnrOrganizationRequestBodyClassDataRow
    {

        private string типField;

        private string видField;

        private string представлениеField;

        private PnrOrganizationRequestBodyClassDataRowЗначенияПолей значенияПолейField;

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
        public PnrOrganizationRequestBodyClassDataRowЗначенияПолей ЗначенияПолей
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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
    public partial class PnrOrganizationRequestBodyClassDataRowЗначенияПолей
    {

        private PnrOrganizationRequestКонтактнаяИнформация контактнаяИнформацияField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
        public PnrOrganizationRequestКонтактнаяИнформация PnrOrganizationRequestКонтактнаяИнформация
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
    public partial class PnrOrganizationRequestКонтактнаяИнформация
    {

        private object комментарийField;

        private PnrOrganizationRequestКонтактнаяИнформацияСостав составField;

        private string представлениеField;

        /// <remarks/>
        public object Комментарий
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
        public PnrOrganizationRequestКонтактнаяИнформацияСостав Состав
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class PnrOrganizationRequestКонтактнаяИнформацияСостав
    {

        private PnrOrganizationRequestКонтактнаяИнформацияСоставСостав составField;

        private string странаField;

        /// <remarks/>
        public PnrOrganizationRequestКонтактнаяИнформацияСоставСостав Состав
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.v8.1c.ru/ssl/contactinfo")]
    public partial class PnrOrganizationRequestКонтактнаяИнформацияСоставСостав
    {

        private string субъектРФField;

        private string улицаField;

        private PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭл[] допАдрЭлField;

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
        public PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭл[] ДопАдрЭл
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
    public partial class PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭл
    {

        private PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭлНомер номерField;

        private uint типАдрЭлField;

        private bool типАдрЭлFieldSpecified;

        private uint значениеField;

        private bool значениеFieldSpecified;

        /// <remarks/>
        public PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭлНомер Номер
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
        public uint ТипАдрЭл
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
        public uint Значение
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
    public partial class PnrOrganizationRequestКонтактнаяИнформацияСоставСоставДопАдрЭлНомер
    {

        private ushort типField;

        private string значениеField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Тип
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
