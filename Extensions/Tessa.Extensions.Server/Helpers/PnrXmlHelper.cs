using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Tessa.Extensions.Server.Helpers
{
    public static class PnrXmlHelper
    {
        public static string ToXML(object value)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        public static T GetObjectFromXml<T>(string xml)
        {
            T message = default(T);
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                message = (T)serializer.Deserialize(reader);
            }
            return message;
        }
    }
}
