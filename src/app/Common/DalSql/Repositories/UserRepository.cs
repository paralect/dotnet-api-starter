using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.DalSql.Repositories;

public class UserRepository : BaseRepository<User, UserFilter>, IUserRepository
{
    public UserRepository(ShipDbContext dbContext) : base(dbContext, dbContext => dbContext.Users)
    {
    }
}
