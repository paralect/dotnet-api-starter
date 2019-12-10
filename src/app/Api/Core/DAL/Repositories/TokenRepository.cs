using Api.Core.DbViews.Token;
using Api.Core.Interfaces.DAL;
using Api.Core.Settings;
using Microsoft.Extensions.Options;

namespace Api.Core.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(IOptions<DbSettings> settings)
            : base(settings, dbContext => dbContext.Tokens)
        { }
    }
}
