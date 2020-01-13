using System;
using Api.Core.Enums;

namespace Api.Core.Services.Infrastructure.Models
{
    public class TokenModel
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
    }
}
