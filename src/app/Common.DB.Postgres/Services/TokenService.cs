using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DB.Postgres.DAL.Documents;
using Common.DB.Postgres.DAL.Interfaces;
using Common.Enums;
using Common.Models;
using Common.Services;
using Common.Settings;
using Common.Utils;
using LinqToDB;
using Microsoft.Extensions.Options;

namespace Common.DB.Postgres.Services
{
    public class TokenService : BaseDocumentService<Token>, ITokenService
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

            await _tokenRepository.InsertRangeAsync(tokens);

            return tokens;
        }

        public async Task<IToken> FindAsync(string tokenValue, TokenTypeEnum type)
        {
            return await _tokenRepository.GetQuery().FirstOrDefaultAsync(x => x.Value == tokenValue && x.Type == type);
        }

        public async Task DeleteUserTokensAsync(string userId)
        {
            await _tokenRepository.DeleteAsync(userId);
        }

        async Task<IToken> IDocumentService<IToken>.FindByIdAsync(string id)
        {
            return await FindByIdAsync(id);
        }
    }
}
