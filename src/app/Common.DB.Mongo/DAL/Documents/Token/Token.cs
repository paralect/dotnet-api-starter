using System;
using Common.Enums;
using Common.Models;

namespace Common.DB.Mongo.DAL.Documents.Token
{
    public class Token : BaseMongoDocument, IToken
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public Guid UserId { get; set; }
    }
}
