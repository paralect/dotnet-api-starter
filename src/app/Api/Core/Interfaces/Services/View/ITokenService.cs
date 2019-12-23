using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.Token;

namespace Api.Core.Interfaces.Services.View
{
    public interface ITokenService : IViewService<Token, TokenFilter>
    {
        Task<List<Token>> CreateAuthTokensAsync(string userId);
        Task<string> FindUserIdByTokenAsync(string tokenValue);
        Task DeleteUserTokensAsync(string userId);
    }
}