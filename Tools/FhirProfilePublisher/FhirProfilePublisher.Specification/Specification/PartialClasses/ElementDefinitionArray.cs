using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public static class ElementDefinitionArray
    {
        public static ElementDefinition GetRootElement(this ElementDefinition[] elements)
        {
            return elements.Single(t => t.path.value.Split('.').Count() == 1);
        }

        public static ElementDefinition[] GetChildren(this ElementDefinition[] elements, ElementDefinition parent)
        {
            return elements.Where(t => IsDirectChildpath(parent.path.value, t.path.value)).ToArray();
        }

        private static bool IsDirectChildpath(string parentpath, string childpath)
        {
            if (!childpath.StartsWith(parentpath + "."))
                return false;

            string childpathWithoutParent = childpath.Remove(0, Math.Min(parentpath.Length + 1, childpath.Length));

            return (!childpathWithoutParent.Contains("."));
        }
    }
}
