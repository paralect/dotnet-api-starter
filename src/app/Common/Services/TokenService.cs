using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Dal.Documents.Token;
using Common.Dal.Interfaces;
using Common.Dal.Repositories;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Services.Interfaces.Models;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace Common.Services;

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

    public async Task<List<Token>> CreateAuthTokensAsync(string userId)
    {
        var accessTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);
        var refreshTokenValue = SecurityUtils.GenerateSecureToken(Constants.TokenSecurityLength);

        var tokens = new List<Token>
            {
                new()
                {
                    Type = TokenType.Access,
                    ExpireAt = DateTime.Now + TimeSpan.FromHours(_tokenExpirationSettings.AccessTokenExpiresInHours),
                    UserId = userId,
                    Value = accessTokenValue
                },
                new()
                {
                    Type = TokenType.Refresh,
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

    public Task<UserTokenModel> GetUserTokenAsync(string accessToken)
    {
        return (from token in _tokenRepository.GetQueryable()
                join user in _userRepository.GetQueryable() on token.UserId equals user.Id
                where token.Type == TokenType.Access && token.Value == accessToken
                select new UserTokenModel
                {
                    UserId = token.UserId,
                    ExpireAt = token.ExpireAt,
                    UserRole = user.Role
                }).FirstOrDefaultAsync();
    }

    public async Task DeleteUserTokensAsync(string userId)
    {
        await _tokenRepository.DeleteManyAsync(new TokenFilter { UserId = userId });
    }
}
