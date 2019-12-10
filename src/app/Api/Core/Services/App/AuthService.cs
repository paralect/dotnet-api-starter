using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.Token;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;

namespace Api.Core.Services.App
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<List<Token>> SetTokens(string userId)
        {
            var tokens = await _tokenService.CreateAuthTokens(userId);

            return tokens;
        }
    }
}
