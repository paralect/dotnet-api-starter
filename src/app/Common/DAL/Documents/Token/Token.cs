using System;
using Common.Enums;
using Common.Utils;

namespace Common.DAL.Documents.Token
{
    public class Token : BaseDocument, IExpirable
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
    }
}
