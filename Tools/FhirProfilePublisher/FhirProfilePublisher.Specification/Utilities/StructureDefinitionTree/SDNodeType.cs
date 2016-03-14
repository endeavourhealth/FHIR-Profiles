using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public enum SDNodeType
    {
        Unknown,
        PrimitiveType,
        ComplexType,
        Element,
        Reference,
        Extension,
        SetupSlice,
        Resource,
        Choice
    }
}
