using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Infrastructure;
using Common;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Infrastructure
{
    public class AuthSqlService : IAuthSqlService
    {
        private readonly ShipDbContext _dbContext;
        
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public AuthSqlService(
            ShipDbContext dbContext,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public void SetTokens(long userId)
        {
            var tokens = GenerateTokens(userId);
            
            _dbContext.Tokens.AddRange(tokens);

            SetCookies(tokens);
        }

        public async Task SetTokensAsync(User user)
        {
            var tokens = GenerateTokens(user.Id);

            var tokensCollection = _dbContext.Entry(user).Collection(u => u.Tokens);
            if (!tokensCollection.IsLoaded)
            {
                await tokensCollection.LoadAsync();
            }

            var newTokens = user.Tokens.Concat(tokens);
            user.Tokens = newTokens.ToList();
            
            SetCookies(tokens);
        }

        public async Task UnsetTokensAsync(long userId)
        {
            var tokens = await _dbContext.Tokens.FindByFilterAsNoTrackingAsync(new TokenFilter
            {
                UserId = userId
            });

            _dbContext.Tokens.RemoveRange(tokens);

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
                    Type = TokenTypeEnum.Access,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_tokenExpirationSettings.AccessTokenExpiresInHours),
                    UserId = userId,
                    Value = accessTokenValue
                },
                new Token
                {
                    Type = TokenTypeEnum.Refresh,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_tokenExpirationSettings.RefreshTokenExpiresInHours),
                    UserId = userId,
                    Value = refreshTokenValue
                }
            };

            return tokens;
        }

        private void SetCookies(ICollection<Token> tokens)
        {
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
    }
}