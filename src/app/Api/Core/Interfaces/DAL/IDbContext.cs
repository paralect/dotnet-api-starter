using Api.Core.DAL.Views.Token;
using Api.Core.DAL.Views.User;
using MongoDB.Driver;

namespace Api.Core.Interfaces.DAL
{
    public interface IDbContext
    {
        IMongoCollection<User> Users { get; }

        IMongoCollection<Token> Tokens { get; }
    }
}
