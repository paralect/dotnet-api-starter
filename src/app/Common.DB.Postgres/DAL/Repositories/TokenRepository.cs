using Common.DB.Postgres.DAL.Documents;
using Common.DB.Postgres.DAL.Interfaces;

namespace Common.DB.Postgres.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(IPostgresDbContext context) : base(context)
        {
        }
    }
}
