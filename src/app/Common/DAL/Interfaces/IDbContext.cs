using Common.DAL.Documents.Token;
using Common.DAL.Documents.User;
using MongoDB.Driver;

namespace Common.DAL.Interfaces
{
    public interface IDbContext
    {
        IMongoClient Client { get; }

        IMongoCollection<User> Users { get; }
        IMongoCollection<Token> Tokens { get; }
    }
}
