using Api.Core.DAL.Views.Token;
using Api.Core.DAL.Views.User;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public class DbContext
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
