using System;
using Common.Enums;

namespace Common.Models
{
    public interface IToken : IEntity
    {
        public TokenTypeEnum Type { get; set; }
        public string? Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public Guid UserId { get; set; }
    }
}
