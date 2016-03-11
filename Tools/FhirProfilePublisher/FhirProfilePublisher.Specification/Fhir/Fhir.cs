using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.V101;

namespace FhirProfilePublisher.Specification
{
    public static class Fhir
    {
        public const string ReferenceTypeName = "Reference";
        public const string BackboneElement = "BackboneElement";
        public const string FlagSymbolIsModifier = "?!";
        public const string FlagSymbolIsSummary = "Σ";
        public const string FlagSymbolHasInvariants = "I";
        public const string DataTypeUrl = "http://hl7.org/fhir/DSTU2/datatypes.html#{0}";
        public const string ReferenceUrl = "http://hl7.org/fhir/DSTU2/references.html";
        public const string BackboneElementUrl = "http://hl7.org/fhir/DSTU2/backboneelement.html";
        public const string ResourceStructureTableHeaderUrl = "http://hl7.org/fhir/DSTU2/formats.html#table";
        public const string BindingStrengthUrl = "http://hl7.org/fhir/DSTU2/terminologies.html#{0}";
        public const string ValueSetUrlPrefixOld = "http://hl7.org/fhir/vs/";
        public const string ValueSetUrlPrefixNew = "http://hl7.org/fhir/ValueSet/";
        public const string ValueSetSourceReferenceExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-sourceReference";
        public const string ValueSetSystemNameExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-systemName";
        public const string ValueSetSystemUrlExtensionUrl = "http://hl7.org/fhir/StructureDefinition/valueset-systemRef";
        public const string ResourceMaturityExtensionUrl = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm";

        public static string GetBindingStrengthUrl(BindingStrengthlist bindingStrength)
        {
            return string.Format(BindingStrengthUrl, bindingStrength.ToString());
        }

        public static string GetDataTypeUrl(string dataType)
        {
            if (FhirData.Instance.IsDataTypeName(dataType))
                return string.Format(DataTypeUrl, dataType);
            else if (IsReference(dataType))
                return ReferenceUrl;
            else if (dataType == BackboneElement)
                return BackboneElementUrl;

            return string.Empty;
        }

        public static bool IsReference(string value)
        {
            return (value == ReferenceTypeName);
        }
        
        public static string NormaliseValueSetUri(string valueSetUri)
        {
            if (valueSetUri != null)
                if (valueSetUri.StartsWith(ValueSetUrlPrefixOld))
                    return valueSetUri.Replace(ValueSetUrlPrefixOld, ValueSetUrlPrefixNew);

            return valueSetUri;
        }

        public static string GetName(this FilterOperatorlist filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperatorlist.Item: return "=";
                case FilterOperatorlist.isa: return "is-a";
                case FilterOperatorlist.isnota: return "is-not-a";
                case FilterOperatorlist.@in: return "in";
                case FilterOperatorlist.notin: return "not-in";
                case FilterOperatorlist.regex: return "regex";
                default: throw new NotSupportedException("Filter operator not recognised");
            }
        }

        public static string GetDescription(this FilterOperatorlist filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperatorlist.Item: return "equals";
                case FilterOperatorlist.isa: return "is, or is a child of";
                case FilterOperatorlist.isnota: return "is not, and is not a child of";
                case FilterOperatorlist.@in: return "is in the set of";
                case FilterOperatorlist.notin: return "is not in the set of";
                case FilterOperatorlist.regex: return "regular expression matches";
                default: throw new NotSupportedException("Filter operator not recognised");
            }
        }

        public static string GetDescription(this BindingStrengthlist? bindingStrength)
        {
            if (bindingStrength == null)
                return string.Empty;

            return StringHelper.UpperCaseFirstCharacter(bindingStrength.Value.ToString());
        }

        public static bool HasExtensionProfile(this ElementDefinitionType type)
        {
            return type.WhenNotNull(t => t.profile.WhenNotNull(s => s.Any()));
        }
    }
}
