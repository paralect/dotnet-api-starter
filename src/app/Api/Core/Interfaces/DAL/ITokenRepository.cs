using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.Token;

namespace Api.Core.Interfaces.DAL
{
    public interface ITokenRepository : IRepository<Token, TokenFilter>
    {
    }
}
