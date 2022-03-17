using System.Linq;
using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.ServicesSql.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.DalSql.Repositories;

public class TokenRepository : BaseRepository<Token, TokenFilter>, ITokenRepository
{
    public TokenRepository(ShipDbContext dbContext) : base(dbContext, dbContext => dbContext.Tokens)
    {
    }

    public async Task<UserTokenModel> FindUserTokenByValueAsync(string value)
    {
        return await dbContext.Tokens
            .Where(x => x.Value == value)
            .Select(x => new UserTokenModel
            {
                UserId = x.UserId,
                UserRole = x.User.Role,
                ExpireAt = x.ExpireAt
            })
            .FirstOrDefaultAsync();
    }
}
