using System;
using Api.Core.Enums;

namespace Api.Core.DAL.Documents.Token
{
    public class Token : BaseDocument
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
    }
}
