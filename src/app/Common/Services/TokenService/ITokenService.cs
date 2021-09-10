using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models;

namespace Common.Services.TokenService
{
    public interface ITokenService : IDocumentService<IToken>
    {
        Task<IEnumerable<IToken>> CreateAuthTokensAsync(string userId);
        Task<IToken> FindAsync(string tokenValue, TokenTypeEnum type);
        Task<UserTokenModel> GetUserTokenAsync(string accessToken);
        Task DeleteUserTokensAsync(string userId);
    }
}