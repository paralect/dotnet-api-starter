using System.Collections.Generic;
using Api.Core.DAL.Views.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Utils;
using MongoDB.Driver;

namespace Api.Core.DAL.Repositories
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

            if (filter.SignupToken.HasValue())
            {
                yield return builder.Eq(u => u.SignupToken, filter.SignupToken);
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
        public string SignupToken { get; set; }
        public string ResetPasswordToken { get; set; }
        public string UserIdToExclude { get; set; }
    }
}
