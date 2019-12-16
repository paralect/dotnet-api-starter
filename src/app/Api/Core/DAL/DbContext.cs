using Api.Core.DbViews.Token;
using Api.Core.DbViews.User;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(Constants.DbDocuments.Users);
        public IMongoCollection<Token> Tokens => _database.GetCollection<Token>(Constants.DbDocuments.Tokens);
    }
}
