using System;
using Common.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DB.Mongo.DAL.Documents
{
    public class BaseMongoDocument : IEntity
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
