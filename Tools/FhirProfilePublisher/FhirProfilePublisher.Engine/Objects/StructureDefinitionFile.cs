using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class StructureDefinitionFile : ResourceFile
    {
        public StructureDefinitionFile(string xml)
        {
            Xml = xml;
            StructureDefinition = XmlHelper.Deserialize<StructureDefinition>(xml);
            Json = JsonConverter.Serialize(StructureDefinition);
        }

        public StructureDefinition StructureDefinition { get; private set; }

        public override OutputFileType FileType
        { 
            get { return OutputFileType.StructureDefinition; } 
        }

        public override string Name 
        { 
            get { return StructureDefinition.GetName(); }
        }

        public override string CanonicalUrl
        {
            get { return StructureDefinition.url.value; }
        }

        public override string OutputHtmlFilename
        {
            get { return StructureDefinition.id.value + (StructureDefinition.IsExtension() ? ".extension" : string.Empty) + "." + HtmlExtension; }
        }

        public override string OutputXmlFilename
        {
            get { return StructureDefinition.id.value + "." + XmlExtension; }
        }

        public override string OutputJsonFilename
        {
            get { return StructureDefinition.id.value + "." + JsonExtension; }
        }

        public override ResourceMaturity Maturity
        {
            get 
            { 
                string value = StructureDefinition.GetExtensionValueAsString(Fhir.ResourceMaturityExtensionUrl);
                return (ResourceMaturity)Utilities.ParseInt(value, default(int)); 
            }
        }
    }
}
