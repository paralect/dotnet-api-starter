using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.DAL.Views
{
    public class BaseView
    {
        [BsonId]
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
