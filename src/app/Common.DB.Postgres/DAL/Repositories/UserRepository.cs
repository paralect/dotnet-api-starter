using Common.DB.Postgres.DAL.Documents;
using Common.DB.Postgres.DAL.Interfaces;

namespace Common.DB.Postgres.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IPostgresDbContext context) : base(context)
        {
        }
    }
}
