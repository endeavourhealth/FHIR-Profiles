using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class ValueSetFile : ResourceFile
    {
        public ValueSetFile(string xml)
        {
            Xml = xml;
            ValueSet = XmlHelper.Deserialize<ValueSet>(xml);
            Json = JsonConverter.Serialize(ValueSet);
        }

        public ValueSet ValueSet { get; private set; }

        public override OutputFileType FileType
        {
            get { return OutputFileType.ValueSet; }
        }

        public override string Name
        {
            get { return ValueSet.name.value; }
        }

        public override string CanonicalUrl 
        { 
            get { return ValueSet.url.value; }
        }

        public override string OutputHtmlFilename
        {
            get { return ValueSet.id.value + ".valueset." + HtmlExtension; }
        }

        public override string OutputXmlFilename
        {
            get { return ValueSet.id.value + "." + XmlExtension; }
        }

        public override string OutputJsonFilename
        {
            get { return ValueSet.id.value + "." + JsonExtension; }
        }

        public override ResourceMaturity Maturity
        {
            get 
            {
                string resourceMaturity = ValueSet.GetExtensionValueAsString(FhirConstants.ResourceMaturityExtensionUrl);

                int result = 0;
                int.TryParse(resourceMaturity, out result);

                return (ResourceMaturity)result;
            }
        }
    }
}
