using Api.Core.DbViews.User;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IOptions<DbSettings> settings)
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(AppConstants.DbDocuments.Users);

        public IMongoCollection<TModel> GetCollection<TModel>(string name)
        {
            return _database.GetCollection<TModel>(name);
        }
    }
}
