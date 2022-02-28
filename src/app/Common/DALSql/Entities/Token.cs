using System;
using Common.Enums;
using Common.Utils;

namespace Common.DALSql.Entities
{
    public class Token : BaseEntity, IExpirable
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public long UserId { get; set; }

        public User User { get; set; }
    }
}