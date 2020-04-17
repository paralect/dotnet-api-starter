using MongoDB.Bson.Serialization.Attributes;

namespace Common.DAL.Documents
{
    public class BaseDocument
    {
        [BsonId]
        public string Id { get; set; }
    }
}
