using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Document;
using Api.Core.Settings;
using Api.Core.Utils;
using Common.DAL.Documents.Token;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.Enums;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Document
{
    public class TokenService : BaseDocumentService<Token, TokenFilter>, ITokenService
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
