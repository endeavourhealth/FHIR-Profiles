using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public interface IStructureDefinitionResolver
    {
        StructureDefinition GetStructureDefinition(string url);
    }
}
