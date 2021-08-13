using Common.DAL.Documents;
using Common.DAL.Interfaces;

namespace Common.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(IDbContext context) : base(context)
        { 
        }
        //public TokenRepository(IDbContext context)
        //    : base(context, idGenerator, dbContext => dbContext.Tokens)
        //{ }

        //protected override IEnumerable<FilterDefinition<Token>> GetFilterQueries(TokenFilter filter)
        //{
        //    var builder = Builders<Token>.Filter;

        //    if (filter.Value.HasValue())
        //    {
        //        yield return builder.Eq(u => u.Value, filter.Value);
        //    }

        //    if (filter.UserId.HasValue())
        //    {
        //        yield return builder.Eq(u => u.UserId, filter.UserId);
        //    }
        //}
    }

    public class TokenFilter : BaseFilter
    {
        public string Value { get; set; }
        public string UserId { get; set; }
    }
}
