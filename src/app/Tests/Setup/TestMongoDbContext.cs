using Api.Core.Abstract;
using Api.Core.Utils;
using Api.Dal;
using Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Setup
{
    public class TestMongoDbContext : MongoDbContext
    {
        public TestMongoDbContext(IOptions<DbSettings> settings)
            : base(settings) { }


        public void DropDatabase()
        {
            _database.Client.DropDatabase(_database.DatabaseNamespace.DatabaseName);
        }
    }
}
