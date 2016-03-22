using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace Hl7.Fhir.V102
{
    public partial class ElementDefinition
    {
        private bool _isFake = false;

        internal bool IsFake
        {
            get
            {
                return _isFake;
            }
            set
            {
                _isFake = value;
            }
        }

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
        
        public string GetCardinalityText()
        {
            int? min = this.min.WhenNotNull(t => t.value);
            string max = this.max.WhenNotNull(t => t.value);

            if (min == null || max == null)
                return null;

            return min.ToString() + ".." + max;
        }

        public string GetLastPathValue()
        {
            return path.value.Split('.').Last();
        }

        public string[] GetInvariantText()
        {
            if (constraint != null)
                return constraint.Select(t => t.human.WhenNotNull(s => s.value)).ToArray();

            return new string[] { };
        }

        public string GetExtensionCanonicalUrl()
        {
            if (type.WhenNotNull(t => t.Length == 1))
                if (type.Single().code.WhenNotNull(t => t.value.ToLower() == "extension"))
                    if (type.WhenNotNull(t => t.Length == 1))
                        if (type.Single().profile.WhenNotNull(s => s.Length == 1))
                            return type.Single().profile.Single().value;

            return null;
        }

        public bool HasFixedValue()
        {
            return (Item1 != null);
        }

        public string GetFixedValue()
        {
            return Item1.GetValueAsString();
        }

        public override string ToString()
        {
            if (path != null)
                return path.value;

            return base.ToString();
        }
    }
}
