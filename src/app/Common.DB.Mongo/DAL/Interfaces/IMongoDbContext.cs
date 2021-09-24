using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Documents.User;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoClient Client { get; }

        IMongoCollection<User> Users { get; }
        IMongoCollection<Token> Tokens { get; }
    }
}
