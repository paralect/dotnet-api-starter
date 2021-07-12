using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        protected TokenRepository(ShipDbContext context) : base(context)
        {
        }

        protected TokenRepository(DbContextOptions<ShipDbContext> options) : base(options)
        {
        }
    }
}