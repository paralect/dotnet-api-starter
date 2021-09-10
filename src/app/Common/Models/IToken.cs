using System;
using Common.Enums;

namespace Common.Models
{
    public interface IToken : IEntity, IExpirable
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public string UserId { get; set; }
    }
}
