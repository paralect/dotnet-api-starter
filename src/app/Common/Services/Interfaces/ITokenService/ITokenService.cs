using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DAL.Documents.Token;
using Common.DAL.Repositories;

namespace Common.Services.Interfaces.ITokenService
{
    public interface ITokenService : IDocumentService<Token, TokenFilter>
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);
        Task<Token> FindAsync(string tokenValue);
        UserToken GetUserToken(string accessToken);
        Task DeleteUserTokensAsync(string userId);
    }
}