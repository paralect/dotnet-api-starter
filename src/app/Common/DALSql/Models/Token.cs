using System;

namespace Common.DALSql.Models
{
    public class Token
    {
        public long TokenId { get; set; }
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
