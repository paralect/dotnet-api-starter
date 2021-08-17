using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Interfaces;
using Common.DB.Mongo.DAL.Repositories;
using Common.Enums;
using Common.Models;
using Common.Services;
using Common.Utils;
using Microsoft.Extensions.Options;

namespace Common.DB.Mongo.Services
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

        public async Task<IEnumerable<IToken>> CreateAuthTokensAsync(string userId)
        {
            var accessTokenValue = SecurityUtils.GenerateSecureToken(Common.Constants.TokenSecurityLength);
            var refreshTokenValue = SecurityUtils.GenerateSecureToken(Common.Constants.TokenSecurityLength);

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

        public async Task<IToken?> FindAsync(string tokenValue, TokenTypeEnum type)
        {
            return await FindOneAsync(new TokenFilter { Value = tokenValue, Type = type });
        }

        public async Task DeleteUserTokensAsync(string userId)
        {
            await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });
        }

        async Task<IToken?> IDocumentService<IToken>.FindByIdAsync(string id)
        {
            return await FindByIdAsync(id);
        }
    }
}
