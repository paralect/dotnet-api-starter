using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Common;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Domain
{
    public class UserSqlService : IUserSqlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IAuthSqlService _authSqlService;
        private readonly TokenExpirationSettings _tokenExpirationSettings;
        private readonly DbSet<User> _users;
        private readonly DbSet<Token> _tokens;

        public UserSqlService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IAuthSqlService authSqlService,
            IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _unitOfWork = unitOfWork;
            _users = unitOfWork.Users;
            _tokens = unitOfWork.Tokens;
            
            _emailService = emailService;
            _authSqlService = authSqlService;
            _tokenExpirationSettings = tokenExpirationSettings.Value;
        }

        public async Task<User> CreateUserAccountAsync(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken
            };

            await _unitOfWork.Perform(() =>
            {
                _users.Add(user);
            });

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return user;
        }

        public async Task VerifyEmail(long userId)
        {
            var user = await _users.FindAsync(userId);
            var tokens = GenerateTokens(userId);

            await _unitOfWork.Perform(() =>
            {
                user.IsEmailVerified = true;
                user.LastRequest = DateTime.UtcNow;

                _tokens.AddRange(tokens);
            });

            _authSqlService.SetTokens(tokens);
        }

        public async Task SignIn(long userId)
        {
            var user = await _users.FindAsync(userId);
            var tokens = GenerateTokens(userId);

            await _unitOfWork.Perform(() =>
            {
                user.LastRequest = DateTime.UtcNow;

                _tokens.AddRange(tokens);
            });
            
            _authSqlService.SetTokens(tokens);
        }

        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await _users.FindAsync(id);

            await _unitOfWork.Perform(() =>
            {
                user.ResetPasswordToken = token;
            });
        }

        public async Task<string> SetResetPasswordToken(long userId)
        {
            var user = await _users.FindAsync(userId);
            if (user.ResetPasswordToken.HasNoValue())
            {
                await _unitOfWork.Perform(() =>
                {
                    user.ResetPasswordToken = SecurityUtils.GenerateSecureToken();
                });
            }

            return user.ResetPasswordToken;
        }

        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await _users.FindAsync(id);
            await _unitOfWork.Perform(() =>
            {
                user.PasswordHash = newPassword.GetHash();
                user.ResetPasswordToken = string.Empty;
            });
        }
        
        public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            var user = await _users.FindAsync(id);
            await _unitOfWork.Perform(() =>
            {
                user.Email = email;
                user.FirstName = firstName;
                user.LastName = lastName;
            });
        }
        
        public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        {
            var user = await _users.FindOneByFilterAsNoTracking(new UserFilter
            {
                IdToExclude = userIdToExclude,
                Email = email
            });
        
            return user != null;
        }

        public IList<Token> GenerateTokens(long userId)
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

            return tokens;
        }

        public async Task SignInGoogleWithCodeAsync(GooglePayloadModel payload)
        {
            var user = await _users.FindOneByFilterAsNoTracking(new UserFilter
            {
                Email = payload.Email
            });
            var tokens = GenerateTokens(user.Id);

            await _unitOfWork.Perform(() =>
            {
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = payload.Email,
                        IsEmailVerified = true,
                        OAuthGoogle = true
                    };

                    _users.Add(user);
                }
                else if (!user.OAuthGoogle)
                {
                    _unitOfWork.Attach(user);

                    user.OAuthGoogle = true;
                    user.LastRequest = DateTime.UtcNow;
                }
                
                _tokens.AddRange(tokens);
            });
            
            _authSqlService.SetTokens(tokens);
        }
    }
}
