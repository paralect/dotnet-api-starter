using Api.Core.DAL.Views.Token;
using Api.Core.Interfaces.DAL;

namespace Api.Core.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(IDbContext context, IIdGenerator idGenerator)
            : base(context, idGenerator, dbContext => dbContext.Tokens)
        { }
    }
}
