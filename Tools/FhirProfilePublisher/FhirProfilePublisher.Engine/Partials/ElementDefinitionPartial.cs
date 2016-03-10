using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Engine;

namespace Hl7.Fhir.V101
{
    public partial class ElementDefinition
    {
        public string GetValueSetUri()
        {
            if (binding != null)
                if (binding.Item != null)
                    if (binding.Item is Reference)
                        return ((Reference)binding.Item).reference.value;

            return null;
        }

        public BindingStrengthlist? GetValueSetBindingStrength()
        {
            if (binding != null)
                if (binding.strength != null)
                    if (binding.strength.valueSpecified)
                        return binding.strength.value;

            return null;
        }

        public string GetW5TopLevelGroup()
        {
            if (mapping == null)
                return string.Empty;

            string w5Group = (mapping
                .FirstOrDefault(t => t.identity.value == "w5")
                .WhenNotNull(t => t.map.value) ?? string.Empty)
                .Split('.')
                .FirstOrDefault();

            if (w5Group == "administrative")
                w5Group = "identification";

            return w5Group;
        }

        public bool IsRemoved()
        {
            int maxCardinality;

            if (int.TryParse(max.WhenNotNull(t => t.value), out maxCardinality))
                return (maxCardinality == 0);

            return false;
        }
        
        public bool IsEmptyExtensionSlice()
        {
            if (path.value.EndsWith(".extension"))
            {
                object[] elements = new object[]
                {
                    name,
                    definition,
                    mapping,
                    type,
                    min,
                    max,
                    alias,
                    comments,
                    binding
                };

                if (elements.Any(t => t != null))
                    return false;

                if (slicing != null)
                    if ((slicing.discriminator != null) && (slicing.rules != null))
                        return true;
            }

            return false;
        }

        public string GetCardinalityText()
        {
            int? min = this.min.WhenNotNull(t => t.value);
            string max = this.max.WhenNotNull(t => t.value);

            if (min == null || max == null)
                return null;

            return min.ToString() + ".." + max;
        }

        public string GetNameFromPath()
        {
            return path.value.Split('.').Last();
        }

        public string[] GetInvariantText()
        {
            if (constraint != null)
                return constraint.Select(t => t.human.WhenNotNull(s => s.value)).ToArray();

            return new string[] { };
        }

        public bool IsExtension()
        {
            if (type.WhenNotNull(t => t.Length == 1))
                if (type.Single().code.WhenNotNull(t => t.value.ToLower() == "extension"))
                    return true;

            return false;
        }

        public string GetExtensionCanonicalUrl()
        {
            if (IsExtension())
                if (type.WhenNotNull(t => t.Length == 1))
                    if (type.Single().profile.WhenNotNull(s => s.Length == 1))
                        return type.Single().profile.Single().value;

            return null;
        }

        public bool IsModifier()
        {
            if (isModifier != null)
                if (isModifier.valueSpecified)
                    return isModifier.value;

            return false;
        }

        public bool IsSummary()
        {
            if (isSummary != null)
                if (isSummary.valueSpecified)
                    return isSummary.value;

            return false;
        }

        public bool HasInvariants()
        {
            if (constraint != null)
                return (constraint.Length > 0);

            return false;
        }

        public bool HasDataTypeChoice()
        {
            if (type != null)
                if (type.Length > 1)
                    if (!AllTypesAreReference())
                        return true;

            return false;
        }

        public string[] GetFlagsSymbols()
        {
            List<string> flags = new List<string>();

            if (IsModifier())
                flags.Add(FhirProfilePublisher.Engine.Fhir.FlagSymbolIsModifier);

            if (IsSummary())
                flags.Add(FhirProfilePublisher.Engine.Fhir.FlagSymbolIsSummary);

            if (HasInvariants())
                flags.Add(FhirProfilePublisher.Engine.Fhir.FlagSymbolHasInvariants);

            return flags.ToArray();
        }

        public bool HasFixedValue()
        {
            return (Item1 != null);
        }

        public string GetFixedValue()
        {
            return Item1.GetValueAsString();
        }

        public bool AllTypesAreReference()
        {
            return type.All(t => t.code.value == FhirProfilePublisher.Engine.Fhir.ReferenceTypeName);
        }
    }
}
