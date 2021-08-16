using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Documents.User;
using Common.DB.Mongo.DAL.Interfaces;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL
{
    public class MongoDbContext : IMongoDbContext
    {
        public MongoDbContext(
            IMongoClient client,
            IMongoCollection<User> users,
            IMongoCollection<Token> tokens
        )
        {
            Client = client;

            Users = users;
            Tokens = tokens;
        }

        public IMongoClient Client { get; }

        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Token> Tokens { get; }
    }
}
