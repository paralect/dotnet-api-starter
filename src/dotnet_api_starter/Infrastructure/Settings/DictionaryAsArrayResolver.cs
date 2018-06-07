using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Settings
{
    public class DictionaryAsArrayResolver : DefaultContractResolver
    {
        private static readonly JsonConverter _converter = new KVPConverter();

        public DictionaryAsArrayResolver() : base()
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType != null && objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return _converter;
            }

            return base.ResolveContractConverter(objectType);
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
            {
                return base.CreateArrayContract(objectType);
            }

            return base.CreateContract(objectType);
        }
    }

    public class KVPConverter : JsonConverter
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
