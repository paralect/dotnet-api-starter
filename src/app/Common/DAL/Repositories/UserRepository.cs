using System.Collections.Generic;
using Common.DAL.Documents.User;
using Common.DAL.Interfaces;
using Common.Utils;
using MongoDB.Driver;

namespace Common.DAL.Repositories
{
    public class UserRepository : BaseRepository<User, UserFilter>, IUserRepository
    {
        public UserRepository(IDbContext context, IIdGenerator idGenerator)
            : base(context, idGenerator, dbContext => dbContext.Users)
        { }

        protected override IEnumerable<FilterDefinition<User>> GetFilterQueries(UserFilter filter)
        {
            var builder = Builders<User>.Filter;

            if (filter.Email.HasValue())
            {
                yield return builder.Eq(u => u.Email, filter.Email);
            }

            if (filter.SignUpToken.HasValue())
            {
                yield return builder.Eq(u => u.SignupToken, filter.SignUpToken);
            }

            if (filter.ResetPasswordToken.HasValue())
            {
                yield return builder.Eq(u => u.ResetPasswordToken, filter.ResetPasswordToken);
            }

            if (filter.UserIdToExclude.HasValue())
            {
                yield return builder.Not(builder.Eq(u => u.Id, filter.UserIdToExclude));
            }
        }
    }

    public class UserFilter : BaseFilter
    {
        public string Email { get; set; }
        public string SignUpToken { get; set; }
        public string ResetPasswordToken { get; set; }
        public string UserIdToExclude { get; set; }
    }
}
