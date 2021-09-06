using System;
using Common.Enums;
using Common.Utils;

namespace Common.Services.Interfaces.Models
{
    public class UserTokenModel : IExpirable
    {
        public string UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public UserRoleEnum UserRole { get; set; }
    }
}
