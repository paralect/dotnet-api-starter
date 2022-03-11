using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Services.Interfaces;
using Common;
using Common.Dal.Documents.Token;
using Common.Dal.Interfaces;
using Common.Dal.Repositories;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly AppSettings _appSettings;
        private readonly TokenExpirationSettings _tokenExpirationSettings;
        private readonly HttpContext _httpContext;

        public AuthService(
            ITokenRepository tokenRepository,
            IOptions<AppSettings> appSettings,
            IOptions<TokenExpirationSettings> tokenExpirationSettings,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _tokenRepository = tokenRepository;
            _appSettings = appSettings.Value;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task SetTokensAsync(string userId, UserRole userRole)
        {
            var accessTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);
            var refreshTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);

            var accessToken = new Token
            {
                Type = TokenType.Access,
                ExpireAt = DateTime.UtcNow + TimeSpan.FromHours(_tokenExpirationSettings.AccessTokenExpiresInHours),
                Value = accessTokenValue,
                UserId = userId,
                UserRole = userRole
            };

            var refreshToken = new Token
            {
                Type = TokenType.Refresh,
                ExpireAt = DateTime.UtcNow + TimeSpan.FromHours(_tokenExpirationSettings.RefreshTokenExpiresInHours),
                Value = refreshTokenValue,
                UserId = userId,
                UserRole = userRole
            };

            await _tokenRepository.InsertManyAsync(new List<Token> { accessToken, refreshToken });

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
            await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });

            _httpContext.Response.Cookies.Delete(Constants.CookieNames.AccessToken);
            _httpContext.Response.Cookies.Delete(Constants.CookieNames.RefreshToken);
        }
    }
}
