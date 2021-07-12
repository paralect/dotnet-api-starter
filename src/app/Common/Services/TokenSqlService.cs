using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Settings;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Common.Services
{
    public class TokenSqlService : ITokenSqlService
    {
        private readonly ShipDbContext _dbContext;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public TokenSqlService(ShipDbContext dbContext, IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _dbContext = dbContext;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public async Task<Token> FindByIdAsync(long id)
        {
            return await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task<List<Token>> CreateAuthTokensAsync(long userId)
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

            _dbContext.Tokens.AddRange(tokens);

            await _dbContext.SaveChangesAsync();

            return tokens;
        }

        public async Task<Token> FindByValueAsync(string tokenValue)
        {
            return await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Value == tokenValue);
        }

        public async Task DeleteUserTokensAsync(long userId)
        {
            var tokens = _dbContext.Tokens.Where(t => t.UserId == userId);
            _dbContext.Tokens.RemoveRange(tokens);

            await _dbContext.SaveChangesAsync();
        }
    }
}