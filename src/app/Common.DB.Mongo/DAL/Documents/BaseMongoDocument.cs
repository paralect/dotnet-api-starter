using System;
using Common.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DB.Mongo.DAL.Documents
{
    public class BaseMongoDocument : IEntity
    {
        public BaseMongoDocument()
        {
            Id = Guid.NewGuid();
        }

        public BaseMongoDocument(Guid id)
        {
            Id = id;
        }
        [BsonId]
        public Guid Id { get; private set; }
    }
}
