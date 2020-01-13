using Api.Core.DAL.Documents.Token;
using Api.Core.DAL.Repositories;

namespace Api.Core.Interfaces.DAL
{
    public interface ITokenRepository : IRepository<Token, TokenFilter>
    {
    }
}
