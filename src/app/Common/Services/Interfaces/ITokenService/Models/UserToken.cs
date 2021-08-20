using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Utils;

namespace Common.Services.Interfaces.ITokenService
{
    public class UserToken : IExpirable
    {
        public string UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public UserRoleEnum UserRole { get; set; }
    }
}
