using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.Abstract
{
    public class BaseModel
    {
        public BaseModel()
        {
            Id = ObjectId.GenerateNewId().ToString(); // TODO move to generator
        }

        [BsonId]
        public string Id { get; set; }
    }
}
