using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Enums;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.App
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenRepository _tokenRepository;
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;

        public AuthService(
            ITokenService tokenService,
            ITokenRepository tokenRepository,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _tokenService = tokenService;
            _tokenRepository = tokenRepository;
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task SetTokens(string userId)
        {
            var tokens = await _tokenService.CreateAuthTokens(userId);
            var accessToken = tokens.Single(t => t.Type == TokenTypeEnum.Access);
            var refreshToken = tokens.Single(t => t.Type == TokenTypeEnum.Refresh);

            var domain = new Uri(_appSettings.WebUrl).Host;

            _httpContext.Response.Cookies.Append(Constants.CookieNames.AccessToken, accessToken.Value, new CookieOptions
            {
                HttpOnly = false,
                Expires = accessToken.ExpireAt,
                Domain = domain
            });

            _httpContext.Response.Cookies.Append(Constants.CookieNames.RefreshToken, refreshToken.Value, new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpireAt,
                Domain = domain
            });
        }

        public async Task UnsetTokens(string userId)
        {
            await _tokenRepository.DeleteMany(t => t.UserId == userId);

            _httpContext.Response.Cookies.Delete(Constants.CookieNames.AccessToken);
            _httpContext.Response.Cookies.Delete(Constants.CookieNames.RefreshToken);
        }
    }
}
