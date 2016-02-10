﻿using FhirProfilePublisher.Converters;
using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                string value = ValueSet.GetExtensionValueAsString(Fhir.ResourceMaturityExtensionUrl);
                return (ResourceMaturity)Utilities.ParseInt(value, default(int));
            }
        }
    }
}