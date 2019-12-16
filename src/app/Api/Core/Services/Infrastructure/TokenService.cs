using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.Token;
using Api.Core.Enums;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Settings;
using Api.Core.Utils;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly AppSettings _appSettings;

        public TokenService(ITokenRepository tokenRepository, IOptions<AppSettings> appSettings)
        {
            _tokenRepository = tokenRepository;
            _appSettings = appSettings.Value;
        }

        public async Task<List<Token>> CreateAuthTokens(string userId)
        {
            var accessTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);
            var refreshTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);

            var tokens = new List<Token>
            {
                new Token
                {
                    Type = TokenTypeEnum.Access,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_appSettings.AccessTokenExpiresInHours),
                    UserId = userId,
                    Value = accessTokenValue
                },
                new Token
                {
                    Type = TokenTypeEnum.Refresh,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_appSettings.RefreshTokenExpiresInHours),
                    UserId = userId,
                    Value = refreshTokenValue
                }
            };

            await _tokenRepository.InsertMany(tokens);

            return tokens;
        }

        public string FindUserIdByToken(string token)
        {
            return _tokenRepository.FindOne(t => t.Value == token)?.UserId;
        }
    }
}
