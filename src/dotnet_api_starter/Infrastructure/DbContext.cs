using dotnet_api_starter.Infrastructure.Settings;
using dotnet_api_starter.Resources.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure
{
    public class DbContext
    {
        private readonly IMongoDatabase _database = null;

        public DbContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);

        }

        public IMongoCollection<UserDocument> Users
        {
            get { return _database.GetCollection<UserDocument>(AppConstants.DbDocuments.Users); }
        }

        public IMongoCollection<TModel> GetCollection<TModel>(string name)
        {
            return _database.GetCollection<TModel>(name);
        }
    }
}
