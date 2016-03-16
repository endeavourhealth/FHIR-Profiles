using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    public class NoIdException : FhirProfilePublisherException
    {
        public NoIdException(string message) : base(message)
        {
        }

        public NoIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
