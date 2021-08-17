using System;
using Common.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DB.Mongo.DAL.Documents
{
    public class BaseMongoDocument : IEntity
    {
        public BaseMongoDocument()
        {
            Id = Guid.NewGuid().ToString();
        }

        public BaseMongoDocument(Guid id)
        {
            Id = id.ToString();
        }
        [BsonId]
        public string Id { get; private set; }
    }
}
