using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;
using Common.DALSql.Repositories;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.Options;

namespace Common.Services
{
    public class TokenSqlService : ITokenSqlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public TokenSqlService(IUnitOfWork unitOfWork, IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _unitOfWork = unitOfWork;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public async Task<Token> FindByIdAsync(long id)
        {
            return await _unitOfWork.Tokens.Find(id);
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

            _unitOfWork.Tokens.AddRange(tokens);

            await _unitOfWork.Complete();

            return tokens;
        }

        public async Task<Token> FindByValueAsync(string value)
        {
            return await _unitOfWork.Tokens.FindByValue(value);
        }

        public async Task DeleteUserTokensAsync(long userId)
        {
            var tokens = await _unitOfWork.Tokens.FindByUserId(userId);
            _unitOfWork.Tokens.DeleteRange(tokens);

            await _unitOfWork.Complete();
        }
    }
}