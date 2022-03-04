using System;
using Common.Enums;
using Common.Utils;

namespace Api.Services.Document.Models
{
    public class UserTokenModel : IExpirable
    {
        public string UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public UserRole UserRole { get; set; }
    }
}
