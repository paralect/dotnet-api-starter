using System;
using Common.Enums;
using Common.Models;

namespace Common.Services.TokenService
{
    public class UserTokenModel : IExpirable
    {
        public string UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public UserRoleEnum UserRole { get; set; }
    }
}