using Api.Core.DAL.Views.User;
using Api.Core.Interfaces.DAL;

namespace Api.Core.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context)
            : base(context, dbContext => dbContext.Users)
        { }
    }
}
