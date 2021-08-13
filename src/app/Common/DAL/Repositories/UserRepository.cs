using Common.DAL.Documents;
using Common.DAL.Interfaces;

namespace Common.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbContext context) : base(context)
        {
        }
    }
}
