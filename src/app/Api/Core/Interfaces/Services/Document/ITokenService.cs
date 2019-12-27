using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DAL.Documents.Token;
using Api.Core.DAL.Repositories;

namespace Api.Core.Interfaces.Services.Document
{
    public interface ITokenService : IDocumentService<Token, TokenFilter>
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);
        Task<string> FindUserIdByTokenAsync(string tokenValue);
        Task DeleteUserTokensAsync(string userId);
    }
}