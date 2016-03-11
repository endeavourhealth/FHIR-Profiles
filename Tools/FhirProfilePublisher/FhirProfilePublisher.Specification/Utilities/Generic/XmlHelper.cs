using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FhirProfilePublisher.Specification
{
    public static class XmlHelper
    {
        public static T Deserialize<T>(string xml) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xml))
                return (T)serializer.Deserialize(reader);
        }

        public static string Serialize<T>(T item)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, item);
                return writer.ToString();
            }
        }

        public static string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static string GetRootNodeName(string xml)
        {
            using (StringReader stringReader = new StringReader(xml))
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    if (xmlReader.MoveToContent() == XmlNodeType.Element)
                        return xmlReader.Name;

            return null;
        }

        public static void Validate(string xml, string[] xsds)
        {
            XDocument xmlDocument;

            using (StringReader reader = new StringReader(xml))
                xmlDocument = XDocument.Load(reader);

            List<XmlSchema> xmlSchemas = new List<XmlSchema>();

            XmlSchemaSet schemas = new XmlSchemaSet();
            
            foreach (string xsd in xsds)
                using (StringReader reader = new StringReader(xsd))
                    schemas.Add(XmlSchema.Read(reader, (sender, e) => { throw new XmlSchemaValidationException(e.Message); }));

            xmlDocument.Validate(schemas, null);
        }
    }
}
