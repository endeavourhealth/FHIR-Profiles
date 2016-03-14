using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class ValueSetCode
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string Definition { get; set; }

        public static ValueSetCode FromValueSetConcept(ValueSetConcept concept)
        {
            return new ValueSetCode()
            {
                Code = concept.code.value,
                DisplayName = concept.display.WhenNotNull(s => s.value),
                Definition = concept.definition.WhenNotNull(s => s.value)
            };
        }

        public static ValueSetCode[] FromValueSetConcept(ValueSetConcept[] concepts)
        {
            return concepts
                .Select(t => ValueSetCode.FromValueSetConcept(t))
                .ToArray();
        }

        public static ValueSetCode FromValueSetConcept1(ValueSetConcept1 concept)
        {
            return new ValueSetCode()
            {
                Code = concept.code.value,
                DisplayName = concept.display.WhenNotNull(s => s.value),
                Definition = string.Empty
            };
        }

        public static ValueSetCode[] FromValueSetConcept1(ValueSetConcept1[] concepts)
        {
            return concepts
                .Select(t => ValueSetCode.FromValueSetConcept1(t))
                .ToArray();
        }
    }
}
