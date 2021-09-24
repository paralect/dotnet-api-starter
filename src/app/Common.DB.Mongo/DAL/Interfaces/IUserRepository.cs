using Common.DB.Mongo.DAL.Documents.User;
using Common.DB.Mongo.DAL.Repositories;

namespace Common.DB.Mongo.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
