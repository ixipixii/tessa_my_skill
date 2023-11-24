using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://esb.axelot.ru")]
    [System.Xml.Serialization.XmlRootAttribute("Message", Namespace = "http://esb.axelot.ru", IsNullable = false)]
    public partial class PnrCFORequest
    {
        private PnrCFORequestBody bodyField;

        private ushort classIdField;

        private System.DateTime creationTimeField;

        private string idField;

        private string sourceField;

        private string typeField;

        /// <remarks/>
        public PnrCFORequestBody Body
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
    public partial class PnrCFORequestBody
    {

        private PnrCFORequestBodyClassData classDataField;

        /// <remarks/>
        public PnrCFORequestBodyClassData classData
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
    public partial class PnrCFORequestBodyClassData
    {

        private string mDM_KeyField;

        private string наименованиеField;

        private string кодField;

        private string видЦФОField;

        private string комментарийField;

        private string используетсяField;

        private string родительField;

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
        public string ВидЦФО
        {
            get
            {
                return this.видЦФОField;
            }
            set
            {
                this.видЦФОField = value;
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
        public string Используется
        {
            get
            {
                return this.используетсяField;
            }
            set
            {
                this.используетсяField = value;
            }
        }

        /// <remarks/>
        public string Родитель
        {
            get
            {
                return this.родительField;
            }
            set
            {
                this.родительField = value;
            }
        }
    }
}