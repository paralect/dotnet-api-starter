using System;
using Common.Utils;

namespace Api.Sql.Models.Account;

public class RefreshTokenModel : IExpirable
{
    public long UserId { get; set; }
    public DateTime ExpireAt { get; set; }
}
