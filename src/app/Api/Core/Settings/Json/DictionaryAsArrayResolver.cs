using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.Core.Settings.Json
{
    public class DictionaryAsArrayResolver : DefaultContractResolver
    {
        private static readonly JsonConverter _converter = new KeyValuePairConverter();

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
}
