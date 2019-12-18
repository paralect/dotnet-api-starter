using Api.Core.DbViews.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Settings;
using Microsoft.Extensions.Options;

namespace Api.Core.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IOptions<DbSettings> settings)
            : base(settings, dbContext => dbContext.Users)
        { }
    }
}
