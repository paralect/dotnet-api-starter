using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Abstract.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoCollectionNameAttribute : Attribute
    {
        public string Name;

        public MongoCollectionNameAttribute(string name)
        {
            Name = name;
        }
    }
}
