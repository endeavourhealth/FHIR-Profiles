using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public static class ElementDefinitionTypeArray
    {
        public static bool AllTypesAreReference(this ElementDefinitionType[] elements)
        {
            return elements.All(t => t.code.value == FhirConstants.ReferenceTypeName);
        }
    }
}
