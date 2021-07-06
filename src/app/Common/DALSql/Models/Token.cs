using System;
using Common.Enums;

namespace Common.DALSql.Models
{
    public class Token
    {
        public long TokenId { get; set; }
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public long UserId { get; set; }

        public User User { get; set; }
    }
}
