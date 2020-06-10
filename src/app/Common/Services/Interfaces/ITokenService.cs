using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DAL.Documents.Token;
using Common.DAL.Repositories;

namespace Common.Services.Interfaces
{
    public interface ITokenService : IDocumentService<Token, TokenFilter>
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);
        Task<string> FindUserIdByTokenAsync(string tokenValue);
        Task DeleteUserTokensAsync(string userId);
    }
}