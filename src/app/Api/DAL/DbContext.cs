using Api.Settings;
using Api.Core.Models.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DAL
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

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>(AppConstants.DbDocuments.Users); }
        }

        public IMongoCollection<TModel> GetCollection<TModel>(string name)
        {
            return _database.GetCollection<TModel>(name);
        }
    }
}
