using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    public class FhirProfilePublisherException : Exception
    {
        public FhirProfilePublisherException(string message) : base(message)
        {
        }

        public FhirProfilePublisherException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FhirProfilePublisherException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
