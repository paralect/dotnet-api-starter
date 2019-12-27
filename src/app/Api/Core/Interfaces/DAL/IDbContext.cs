using Api.Core.DAL.Documents.Token;
using Api.Core.DAL.Documents.User;
using MongoDB.Driver;

namespace Api.Core.Interfaces.DAL
{
    public interface IDbContext
    {
        IMongoCollection<User> Users { get; }

        IMongoCollection<Token> Tokens { get; }
    }
}
