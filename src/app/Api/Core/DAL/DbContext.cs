using Api.Core.DAL.Views.Token;
using Api.Core.DAL.Views.User;
using Api.Core.Interfaces.DAL;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public class DbContext : IDbContext
    {
        public DbContext(IMongoCollection<User> users, IMongoCollection<Token> tokens)
        {
            Users = users;
            Tokens = tokens;
        }

        public IMongoCollection<User> Users { get; }

        public IMongoCollection<Token> Tokens { get; }
    }
}
