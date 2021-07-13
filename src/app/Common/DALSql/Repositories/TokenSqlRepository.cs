using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Repositories
{
    public class TokenSqlRepository : BaseRepository<Token>, ITokenSqlRepository
    {
        public TokenSqlRepository(ShipDbContext context) : base(context)
        {
        }

        protected TokenSqlRepository(DbContextOptions<ShipDbContext> options) : base(options)
        {
        }

        public async Task<Token> FindByValue(string value)
        {
            return await Table.FirstOrDefaultAsync(t => t.Value == value);
        }

        public async Task<IEnumerable<Token>> FindByUserId(long userId)
        {
            return await Table.Where(t => t.UserId == userId).ToListAsync();
        }
    }
}