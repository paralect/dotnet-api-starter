using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.DAL.Documents
{
    public class BaseDocument
    {
        [BsonId]
        public string Id { get; set; }
    }
}
