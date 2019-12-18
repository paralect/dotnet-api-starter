using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.Token;

namespace Api.Core.Interfaces.Services.Infrastructure
{
    public interface ITokenService
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);

        string FindUserIdByToken(string token);
    }
}