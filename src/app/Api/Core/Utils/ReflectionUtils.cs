using Api.Core.Abstract.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Utils
{
    public static class ReflectionUtils
    {
        public static string GetMongoCollectionName(Type type)
        {
            var nameAttr = (MongoCollectionNameAttribute)type.GetCustomAttributes(typeof(MongoCollectionNameAttribute), false).FirstOrDefault();
            
            if (nameAttr == null)
            {
                throw new ArgumentException($"Type '{type.FullName}' doesn't have MongoCollectionNameAttribute assigned.");
            }

            return nameAttr.Name;
        }
    }
}
