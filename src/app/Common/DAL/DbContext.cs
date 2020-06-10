using Common.DAL.Documents.Token;
using Common.DAL.Documents.User;
using Common.DAL.Interfaces;
using MongoDB.Driver;

namespace Common.DAL
{
    public class DbContext : IDbContext
    {
        public DbContext(
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
