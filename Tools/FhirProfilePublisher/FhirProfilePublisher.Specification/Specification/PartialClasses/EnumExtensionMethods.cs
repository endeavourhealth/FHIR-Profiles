using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public static class EnumExtensionMethods
    {
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

        public static string GetUrl(this BindingStrengthlist? bindingStrength)
        {
            if (bindingStrength == null)
                return string.Empty;

            return string.Format(FhirConstants.BindingStrengthUrl, bindingStrength.Value.ToString());
        }
    }
}
