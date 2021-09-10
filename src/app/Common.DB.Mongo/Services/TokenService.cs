using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents.Token;
using Common.DB.Mongo.DAL.Interfaces;
using Common.DB.Mongo.DAL.Repositories;
using Common.Enums;
using Common.Models;
using Common.Services;
using Common.Services.TokenService;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace Common.DB.Mongo.Services
{
    public class TokenService : BaseDocumentService<Token, TokenFilter>, ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository, IOptions<TokenExpirationSettings> tokenExpirationSettings)
            : base(tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
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

        public async Task<IToken> FindAsync(string tokenValue, TokenTypeEnum type)
        {
            return await FindOneAsync(new TokenFilter { Value = tokenValue, Type = type });
        }

        public async Task DeleteUserTokensAsync(string userId)
        {
            await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });
        }

        public Task<UserTokenModel> GetUserTokenAsync(string accessToken)
        {
            return (from token in _tokenRepository.GetQueryable()
                    join user in _userRepository.GetQueryable() on token.UserId equals user.Id
                    where token.Type == TokenTypeEnum.Access && token.Value == accessToken
                    select new UserTokenModel
                    {
                        UserId = token.UserId,
                        ExpireAt = token.ExpireAt,
                        UserRole = user.Role
                    }).FirstOrDefaultAsync();
        }

        async Task<IToken> IDocumentService<IToken>.FindByIdAsync(string id)
        {
            return await FindByIdAsync(id);
        }
    }
}
