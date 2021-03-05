using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DAL.Documents.Token;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.Options;

namespace Common.Services
{
    public class TokenService : BaseDocumentService<Token, TokenFilter>, ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public TokenService(ITokenRepository tokenRepository, IOptions<TokenExpirationSettings> tokenExpirationSettings)
            : base(tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public async Task<List<Token>> CreateAuthTokensAsync(string userId)
        {
            var accessTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);
            var refreshTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);

            var tokens = new List<Token>
            {
                new()
                {
                    Type = TokenTypeEnum.Access,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_tokenExpirationSettings.AccessTokenExpiresInHours),
                    UserId = userId,
                    Value = accessTokenValue
                },
                new()
                {
                    Type = TokenTypeEnum.Refresh,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_tokenExpirationSettings.RefreshTokenExpiresInHours),
                    UserId = userId,
                    Value = refreshTokenValue
                }
            };

            await _tokenRepository.InsertManyAsync(tokens);

            return tokens;
        }

        public async Task<Token> FindAsync(string tokenValue)
        {
            return await FindOneAsync(new TokenFilter { Value = tokenValue });
        }

        public async Task DeleteUserTokensAsync(string userId)
        {
            await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });
        }
    }
}
