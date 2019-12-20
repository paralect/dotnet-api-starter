using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.User;

namespace Api.Core.Interfaces.DAL
{
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
