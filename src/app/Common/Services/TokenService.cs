using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Settings;
using Common.Utils;
using LinqToDB;
using Microsoft.Extensions.Options;

namespace Common.Services
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

        public async Task<List<Token>> CreateAuthTokensAsync(long userId)
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

        public Task<Token?> FindAsync(string tokenValue, TokenTypeEnum type)
        {
            return _tokenRepository.GetQuery().FirstOrDefaultAsync(x => x.Type == type && x.Value == tokenValue);
        }

        public async Task DeleteUserTokensAsync(long userId)
        {
            await _tokenRepository.DeleteAsync(userId);
        }
    }
}
