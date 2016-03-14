using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    public class ReferenceNotFoundException : FhirProfilePublisherException
    {
        public ReferenceNotFoundException(string message) : base(message)
        {
        }

        public ReferenceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReferenceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
