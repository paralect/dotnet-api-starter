using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Repositories;

namespace Common.DB.Mongo.DAL.Interfaces
{
    public interface ITokenRepository : IRepository<Token, TokenFilter>
    {
    }
}
