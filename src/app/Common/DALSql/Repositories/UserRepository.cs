using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        protected UserRepository(ShipDbContext context) : base(context)
        {
        }

        protected UserRepository(DbContextOptions<ShipDbContext> options) : base(options)
        {
        }
    }
}