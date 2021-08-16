using System;
using System.Collections.Generic;
using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Interfaces;
using Common.Enums;
using Common.Utils;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token, TokenFilter>, ITokenRepository
    {
        public TokenRepository(IMongoDbContext context)
            : base(context, dbContext => dbContext.Tokens)
        { }

        protected override IEnumerable<FilterDefinition<Token>> GetFilterQueries(TokenFilter filter)
        {
            var builder = Builders<Token>.Filter;

            if (filter.Value.HasValue())
            {
                yield return builder.Eq(u => u.Value, filter.Value);
            }

            if (filter.UserId.HasValue)
            {
                yield return builder.Eq(u => u.UserId, filter.UserId);
            }
        }
    }

    public class TokenFilter : BaseFilter
    {
        public string Value { get; set; }
        public TokenTypeEnum Type { get; set; }
        public Guid? UserId { get; set; }
    }
}
