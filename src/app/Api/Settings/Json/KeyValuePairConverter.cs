using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Settings.Json
{
    public class KeyValuePairConverter : JsonConverter
    {
        private readonly Type _type = typeof(KeyValuePair<,>);
        
        public override bool CanRead
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            object key = value.GetType().GetProperty("Key").GetValue(value);
            object val = value.GetType().GetProperty("Value").GetValue(value);

            writer.WriteStartObject();
            writer.WritePropertyName(key.ToString().ToLower());
            serializer.Serialize(writer, val);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == _type;
        }
    }
}
