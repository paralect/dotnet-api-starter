using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.Token;
using Api.Core.Enums;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.View;
using Api.Core.Settings;
using Api.Core.Utils;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.View
{
    public class TokenService : BaseViewService<Token, TokenFilter>, ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly AppSettings _appSettings;

        public TokenService(ITokenRepository tokenRepository, IOptions<AppSettings> appSettings)
            : base(tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _appSettings = appSettings.Value;
        }

        public async Task<List<Token>> CreateAuthTokensAsync(string userId)
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

            await _tokenRepository.InsertManyAsync(tokens);

            return tokens;
        }

        public async Task<string> FindUserIdByTokenAsync(string tokenValue)
        {
            var token = await FindOneAsync(new TokenFilter { Value = tokenValue });
            return token?.UserId;
        }

        public async Task DeleteUserTokensAsync(string userId)
        {
            await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });
        }
    }
}
