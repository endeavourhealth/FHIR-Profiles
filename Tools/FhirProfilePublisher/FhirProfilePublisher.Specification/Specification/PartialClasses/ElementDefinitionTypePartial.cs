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
            return FhirData.Instance.ComplexDataTypeNames.Contains(code.value) && (!IsExtension());
        }

        public bool IsReference()
        {
            return (code.value == FhirConstants.ReferenceTypeName);
        }

        public bool IsExtension()
        {
            return (code.value == FhirConstants.ExtensionTypeName);
        }

        public string TypeName
        {
            get
            {
                return code.value;
            }
        }
    }
}
