using FhirProfilePublisher.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.V101
{
    public partial class ElementDefinitionType
    {
        public bool IsPrimitiveType()
        {
            return FhirData.Instance.PrimitiveDataTypeNames.Contains(code.value);
        }

        public bool IsComplexType()
        {
            return FhirData.Instance.ComplexDataTypeNames.Contains(code.value);
        }

        public bool IsReference()
        {
            return (code.value == FhirConstants.ReferenceTypeName);
        }
    }
}
