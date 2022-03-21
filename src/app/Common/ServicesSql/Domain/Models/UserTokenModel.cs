using System;
using Common.Enums;
using Common.Utils;

namespace Common.ServicesSql.Domain.Models;

public class UserTokenModel : IExpirable
{
    public long UserId { get; set; }
    public UserRole UserRole { get; set; }
    public DateTime ExpireAt { get; set; }
}
