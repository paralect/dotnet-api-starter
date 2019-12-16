using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.DbViews
{
    public class BaseView
    {
        public BaseView()
        {
            Id = ObjectId.GenerateNewId().ToString(); // TODO move to generator
        }

        [BsonId]
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
