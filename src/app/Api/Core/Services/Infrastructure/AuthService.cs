using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Infrastructure;
using Common;
using Common.Enums;
using Common.Services;
using Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;

        public AuthService(
            ITokenService tokenService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _tokenService = tokenService;
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task SetTokensAsync(string userId)
        {
            var tokens = await _tokenService.CreateAuthTokensAsync(userId);
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

        public async Task UnsetTokensAsync(string userId)
        {
            await _tokenService.DeleteUserTokensAsync(userId);

            _httpContext.Response.Cookies.Delete(Constants.CookieNames.AccessToken);
            _httpContext.Response.Cookies.Delete(Constants.CookieNames.RefreshToken);
        }
    }
}
