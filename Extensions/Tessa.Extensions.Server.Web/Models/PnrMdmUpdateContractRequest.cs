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
    public partial class PnrMdmUpdateContractRequest
    {

        private PnrMdmUpdateContractRequestBody bodyField;

        private ushort classIdField;

        private System.DateTime creationTimeField;

        private string idField;

        private string sourceField;

        private string typeField;

        /// <remarks/>
        public PnrMdmUpdateContractRequestBody Body
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
    public partial class PnrMdmUpdateContractRequestBody
    {

        private PnrMdmUpdateContractRequestBodyClassData classDataField;

        /// <remarks/>
        public PnrMdmUpdateContractRequestBodyClassData classData
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
    public partial class PnrMdmUpdateContractRequestBodyClassData
    {

        private string mDM_KeyField;

        private string кодField;

        private string наименованиеField;

        private string номерДоговораField;

        private System.DateTime? датаДоговораField;

        private string валютаРасчетовField;

        private string цФОField;

        private string проектField;

        private string основнаяСтатьяОборотовField;

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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.DateTime? ДатаДоговора
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


}
