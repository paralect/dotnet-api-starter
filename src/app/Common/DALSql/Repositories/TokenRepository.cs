using Common.DALSql.Entities;
using Common.DALSql.Filters;

namespace Common.DALSql.Repositories
{
    public class TokenRepository : BaseRepository<Token, TokenFilter>, ITokenRepository
    {
        public TokenRepository(ShipDbContext dbContext) : base(dbContext, dbContext => dbContext.Tokens)
        {
        }
    }
}
