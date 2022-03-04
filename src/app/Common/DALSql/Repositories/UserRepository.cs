using Common.DALSql.Entities;
using Common.DALSql.Filters;

namespace Common.DALSql.Repositories
{
    public class UserRepository : BaseRepository<User, UserFilter>, IUserRepository
    {
        public UserRepository(ShipDbContext dbContext) : base(dbContext, dbContext => dbContext.Users)
        {
        }
    }
}
