using Common.DALSql.Entities;
using Common.DALSql.Filters;

namespace Common.DALSql.Repositories
{
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
