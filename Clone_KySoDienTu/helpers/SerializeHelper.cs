using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Clone_KySoDienTu.Helpers
{
    public static class SerializeHelper
    {
        public static string SerializeToXmlString<T>(T data)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            var xmlSettings = new XmlWriterSettings { OmitXmlDeclaration = true };

            using (var xmlWriter = XmlWriter.Create(stringWriter, xmlSettings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", ""); // Bỏ xmlns
                xmlSerializer.Serialize(xmlWriter, data, ns);
                return stringWriter.ToString();
            }
        }
    }
}