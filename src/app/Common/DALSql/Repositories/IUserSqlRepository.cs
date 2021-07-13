using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Common.DALSql.Repositories
{
    public interface IUserSqlRepository : IRepository<User>
    {
        Task<User> FindByEmail(string email);
    }
}