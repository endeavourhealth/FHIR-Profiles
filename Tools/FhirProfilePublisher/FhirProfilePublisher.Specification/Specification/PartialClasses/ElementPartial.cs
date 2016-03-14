using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace Hl7.Fhir.V102
{
    public partial class Element
    {
        public string GetExtensionValueAsString(string extensionUrl)
        {
            if (extension == null)
                return null;

            return extension
                .FirstOrDefault(t => t.url == extensionUrl)
                .WhenNotNull(t => t.Item.GetValueAsString());
        }

        public string GetValueAsString()
        {
            Type elementType = GetType();

            if (elementType == typeof(uri))
                return (this as uri).value;
            else if (elementType == typeof(@string))
                return (this as @string).value;
            else if (elementType == typeof(code))
                return (this as code).value;
            else if (elementType == typeof(integer))
                return (this as integer).value.ToString();

            throw new NotImplementedException("Element");
        }
    }
}
