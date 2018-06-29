using Api.Settings;
using Api.Core.Models.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Api.Core.Abstract;
using Api.Core.Utils;

namespace Api.Dal
{
    public class MongoDbContext : IMongoDbContext
    {
        protected readonly IMongoDatabase _database = null;

        public MongoDbContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);

            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }
        

        public IMongoCollection<TModel> GetCollection<TModel>()
        {
            return _database.GetCollection<TModel>(ReflectionUtils.GetMongoCollectionName(typeof(TModel)));
        }
    }
}
