using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.DalSql.Repositories;

public interface IUserRepository : IRepository<User, UserFilter>
{
}
