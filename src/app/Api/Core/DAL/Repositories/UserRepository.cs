using Api.Core.DAL.Views.User;
using Api.Core.Interfaces.DAL;

namespace Api.Core.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbContext context, IIdGenerator idGenerator)
            : base(context, idGenerator, dbContext => dbContext.Users)
        { }
    }
}
