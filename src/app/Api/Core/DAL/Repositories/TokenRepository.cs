using System;
using Api.Core.DbViews.Token;
using Api.Core.Interfaces.DAL;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Core.DAL.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(
            IOptions<DbSettings> settings,
            Func<DbContext, IMongoCollection<Token>> collectionProvider)
            : base(settings, collectionProvider)
        {
        }
    }
}
