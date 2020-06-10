using Common.DAL.Documents.Token;
using Common.DAL.Repositories;

namespace Common.DAL.Interfaces
{
    public interface ITokenRepository : IRepository<Token, TokenFilter>
    {
    }
}
