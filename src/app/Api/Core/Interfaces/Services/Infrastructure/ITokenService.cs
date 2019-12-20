using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DAL.Views.Token;

namespace Api.Core.Interfaces.Services.Infrastructure
{
    public interface ITokenService
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);

        Task<string> FindUserIdByTokenAsync(string tokenValue);
    }
}