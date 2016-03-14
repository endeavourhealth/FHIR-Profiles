using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.V102;

namespace FhirProfilePublisher.Specification
{
    public static class FhirConstants
    {
        public const string ReferenceTypeName = "Reference";
        public const string ExtensionTypeName = "Extension";
        public const string BackboneElement = "BackboneElement";

        public const string DataTypeCanonicalUrl = "http://hl7.org/fhir/StructureDefinition/{0}";
        public const string ValueSetUrlPrefixOld = "http://hl7.org/fhir/vs/";
        public const string ValueSetUrlPrefixNew = "http://hl7.org/fhir/ValueSet/";
        public const string ValueSetSourceReferenceExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-sourceReference";
        public const string ValueSetSystemNameExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-systemName";
        public const string ValueSetSystemUrlExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-systemRef";
        public const string ResourceMaturityExtensionUrl = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm";

        public const string DataTypeUrl = "http://hl7.org/fhir/DSTU2/datatypes.html#{0}";
        public const string ReferenceUrl = "http://hl7.org/fhir/DSTU2/references.html";
        public const string BackboneElementUrl = "http://hl7.org/fhir/DSTU2/backboneelement.html";
        public const string ResourceStructureTableHeaderUrl = "http://hl7.org/fhir/DSTU2/formats.html#table";
        public const string BindingStrengthUrl = "http://hl7.org/fhir/DSTU2/terminologies.html#{0}";
    }
}
