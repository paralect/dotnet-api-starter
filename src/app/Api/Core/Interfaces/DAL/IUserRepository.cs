using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.User;

namespace Api.Core.Interfaces.DAL
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsers();
    }
}
