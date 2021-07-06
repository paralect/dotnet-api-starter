using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Models;
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
        private readonly ShipContext _context;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public TokenSqlService(ShipContext context, IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _context = context;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public async Task<Token> FindByIdAsync(long id)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == id);
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

            _context.Tokens.AddRange(tokens);

            await _context.SaveChangesAsync();

            return tokens;
        }

        public async Task<Token> FindByValueAsync(string tokenValue)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.Value == tokenValue);
        }

        public async Task DeleteUserTokensAsync(long userId)
        {
            var tokens = _context.Tokens.Where(t => t.UserId == userId);
            _context.Tokens.RemoveRange(tokens);

            await _context.SaveChangesAsync();
        }
    }
}