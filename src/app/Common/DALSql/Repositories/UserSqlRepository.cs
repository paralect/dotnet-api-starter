using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Repositories
{
    public class UserSqlRepository : BaseRepository<User>, IUserSqlRepository
    {
        public UserSqlRepository(ShipDbContext context) : base(context)
        {
        }

        protected UserSqlRepository(DbContextOptions<ShipDbContext> options) : base(options)
        {
        }

        public async Task<User> FindByEmail(string email)
        {
            return await Table.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}