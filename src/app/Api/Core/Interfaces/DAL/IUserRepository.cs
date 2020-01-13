using Api.Core.DAL.Documents.User;
using Api.Core.DAL.Repositories;

namespace Api.Core.Interfaces.DAL
{
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
