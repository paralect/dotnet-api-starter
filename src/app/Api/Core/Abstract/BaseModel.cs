using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public class BaseModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
