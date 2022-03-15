using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Repositories;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Services.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;
        private readonly TokenExpirationSettings _tokenExpirationSettings;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IOptions<TokenExpirationSettings> tokenExpirationSettings,
            ITokenRepository tokenRepository)
        {
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
            _tokenRepository = tokenRepository;
        }

        public async Task SetTokensAsync(long userId)
        {
            var tokens = GenerateTokens(userId);

            await _tokenRepository.InsertManyAsync(tokens);

            SetCookies(tokens);
        }

        public void SetTokens(User user)
        {
            var tokens = GenerateTokens(user.Id);

            var newTokens = user.Tokens.Concat(tokens);
            user.Tokens = newTokens.ToList();

            SetCookies(tokens);
        }

        public async Task UnsetTokensAsync(long userId)
        {
            await _tokenRepository.DeleteManyAsync(new TokenFilter
            {
                UserId = userId
            });

            _httpContext.Response.Cookies.Delete(Constants.CookieNames.AccessToken);
            _httpContext.Response.Cookies.Delete(Constants.CookieNames.RefreshToken);
        }

        private IList<Token> GenerateTokens(long userId)
        {
            var accessTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);
            var refreshTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);

            var tokens = new List<Token>
            {
                new Token
                {
                    Type = TokenType.Access,
                    ExpireAt = DateTime.UtcNow + TimeSpan.FromHours(_tokenExpirationSettings.AccessTokenExpiresInHours),
                    UserId = userId,
                    Value = accessTokenValue
                },
                new Token
                {
                    Type = TokenType.Refresh,
                    ExpireAt = DateTime.UtcNow + TimeSpan.FromHours(_tokenExpirationSettings.RefreshTokenExpiresInHours),
                    UserId = userId,
                    Value = refreshTokenValue
                }
            };

            return tokens;
        }

        private void SetCookies(ICollection<Token> tokens)
        {
            var accessToken = tokens.Single(t => t.Type == TokenType.Access);
            var refreshToken = tokens.Single(t => t.Type == TokenType.Refresh);

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
    }
}