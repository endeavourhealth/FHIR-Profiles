using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FhirProfilePublisher.Specification
{
    internal class CustomContractResolver : DefaultContractResolver
    {
        public CustomContractResolver()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // skip XmlIgnore attributes
            if (Attribute.IsDefined(member, typeof(XmlIgnoreAttribute), true))
                property.Ignored = true;
            
            return property;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (properties != null)
                return properties.OrderBy(p => BaseTypesAndSelf(p.DeclaringType).Count()).ToList();

            return properties;
        }

        private static IEnumerable<Type> BaseTypesAndSelf(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
