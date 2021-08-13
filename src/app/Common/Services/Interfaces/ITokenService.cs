using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Repositories;
using Common.Enums;

namespace Common.Services.Interfaces
{
    public interface ITokenService : IDocumentService<Token>
    {
        Task<List<Token>> CreateAuthTokensAsync(long userId);
        Task<Token?> FindAsync(string tokenValue, TokenTypeEnum type);
        Task DeleteUserTokensAsync(long userId);
    }
}