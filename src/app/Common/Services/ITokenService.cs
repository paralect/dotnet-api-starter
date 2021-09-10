using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models;

namespace Common.Services
{
    public interface ITokenService : IDocumentService<IToken>
    {
        Task<IEnumerable<IToken>> CreateAuthTokensAsync(string userId);
        Task<IToken> FindAsync(string tokenValue, TokenTypeEnum type);
        Task DeleteUserTokensAsync(string userId);
    }
}