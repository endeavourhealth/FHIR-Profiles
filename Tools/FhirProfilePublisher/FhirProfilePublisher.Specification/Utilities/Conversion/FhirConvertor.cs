using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    internal class FhirConvertor : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                // ignore null values
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CustomContractResolver(),

                // format output
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            JsonSerializer serializer2 = JsonSerializer.Create(settings);
            
            JToken token = JObject.FromObject(value, serializer2);

            // flatten value properties
            RemoveValueProperties(token);

            token.WriteTo(writer);
        }

        private void RemoveValueProperties(JToken node)
        {
            Stack<JToken> stack = new Stack<JToken>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                JToken current = stack.Pop();

                if (HasSingleChildProperty(current))
                {
                    if ((current.First as JProperty).Name == "value")
                        current.Replace((current.First as JProperty).Value);
                }
                else
                {
                    foreach (JToken token in current.Children())
                        stack.Push(token);
                }
            }
        }

        private bool HasSingleChildProperty(JToken jObject)
        {
            if (jObject != null)
                if (jObject is JObject)
                    if (jObject.Children().Count() == 1)
                        if (jObject.First is JProperty)
                            if ((jObject.First as JProperty).Value is JValue)
                                return true;

            return false;
        }
    }
}
