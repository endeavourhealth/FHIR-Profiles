using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace Hl7.Fhir.V102
{
    public partial class StructureDefinition
    {
        public bool IsExtension()
        {
            return (constrainedType.value.ToLower() == "extension");
        }

        public string GetTypeDescription()
        {
            if (IsExtension())
                return "Extension";

            return StringHelper.UpperCaseFirstCharacter(kind.value.ToString());
        }

        public string GetName()
        {
            return name.value;
        }

        public string GetDisplayName()
        {
            return (display ?? name).value;
        }

        public ElementDefinitionType[] GetSimpleExtensionType()
        {
            if (!IsExtension())
                throw new ArgumentException("Not an extension definition", "extensionDefinition");

            ElementDefinition element = differential.element.FirstOrDefault(t => t.path.value == "Extension.value[x]");

            if (element == null)
                return null;

            if (element.max.WhenNotNull(t => t.value) == "0")
                return null;

            return element.type;
        }

        public string GetExtensionValueAsString(string url)
        {
            if (extension == null)
                return null;

            Extension ext = extension.FirstOrDefault(t => t.url == url);

            return ext.Item.GetValueAsString();
        }

        public ElementDefinition GetRootPathElement()
        {
            return differential
                .element
                .Single(t => t.path.value.Split('.').Count() == 1);
        }

        public string W5TopLevelGroup
        {
            get
            {
                string baseUrl = @base.value;

                StructureDefinition baseStructureDefinition = FhirData.Instance.FindStructureDefinition(baseUrl);
                ElementDefinition elementDefinition = GetRootPathElement();

                return StringHelper.UpperCaseFirstCharacter(elementDefinition.GetW5TopLevelGroup());
            }
        }

        public override string ToString()
        {
            return name.WhenNotNull(t => t.value);
        }
    }
}
