using System;
using Common.Enums;

namespace Api.NoSql.Services.Models
{
    public class TokenModel
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
    }
}
