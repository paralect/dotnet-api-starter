using Common.DAL.Documents;
using Common.DAL.Interfaces;

namespace Common.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(IDbContext context) : base(context)
        {
        }
    }
}
