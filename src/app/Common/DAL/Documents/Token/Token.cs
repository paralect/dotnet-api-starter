using System;
using Common.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DAL.Documents.Token
{
    public class Token : BaseDocument
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
        
        [BsonIgnore]
        public bool IsExpired => ExpireAt <= DateTime.UtcNow;
    }
}
