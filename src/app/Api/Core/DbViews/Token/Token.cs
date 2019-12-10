using System;
using Api.Core.Enums;

namespace Api.Core.DbViews.Token
{
    public class Token : BaseView
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
