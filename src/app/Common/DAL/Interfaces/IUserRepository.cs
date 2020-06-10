using Common.DAL.Documents.User;
using Common.DAL.Repositories;

namespace Common.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
