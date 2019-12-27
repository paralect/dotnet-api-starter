using Api.Core.DAL.Documents.Token;
using Api.Core.DAL.Documents.User;
using Api.Core.Interfaces.DAL;
using MongoDB.Driver;

namespace Api.Core.DAL
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
