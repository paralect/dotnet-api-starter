using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Common;
using Common.DALSql.Entities;
using Common.DALSql.Repositories;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Domain
{
    public class UserSqlService : IUserSqlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IAuthSqlService _authSqlService;
        private readonly TokenExpirationSettings _tokenExpirationSettings;

        public UserSqlService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IAuthSqlService authSqlService,
            IOptions<TokenExpirationSettings> tokenExpirationSettings)
        {
            _unitOfWork = unitOfWork;
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

            _unitOfWork.Users.Add(user);
            await _unitOfWork.Complete();
            
            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return user;
        }

        public async Task VerifyEmail(long userId)
        {
            var user = await _unitOfWork.Users.Find(userId);
            user.IsEmailVerified = true;
            user.LastRequest = DateTime.UtcNow;

            var tokens = GenerateTokens(userId);
            _unitOfWork.Tokens.AddRange(tokens);

            await _unitOfWork.Complete();

            _authSqlService.SetTokens(tokens);
        }

        public async Task SignIn(long userId)
        {
            var user = await _unitOfWork.Users.Find(userId);
            user.LastRequest = DateTime.UtcNow;
            
            var tokens = GenerateTokens(userId);
            _unitOfWork.Tokens.AddRange(tokens);

            await _unitOfWork.Complete();
            
            _authSqlService.SetTokens(tokens);
        }


        //
        //
        // public async Task UpdateLastRequestAsync(long id)
        // {
        //     var user = await FindByIdAsync(id);
        //     user.LastRequest = DateTime.UtcNow;
        //
        //     await _dbContext.SaveChangesAsync();
        // }
        //
        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await _unitOfWork.Users.Find(id);
            user.ResetPasswordToken = token;
        
            await _unitOfWork.Complete();
        }

        public async Task<string> SetResetPasswordToken(long userId)
        {
            var user = await _unitOfWork.Users.Find(userId);
            if (user.ResetPasswordToken.HasNoValue())
            {
                user.ResetPasswordToken = SecurityUtils.GenerateSecureToken();
                await _unitOfWork.Complete();
            }

            return user.ResetPasswordToken;
        }
        //
        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await _unitOfWork.Users.Find(id);
            user.PasswordHash = newPassword.GetHash();
            user.ResetPasswordToken = string.Empty;

            await _unitOfWork.Complete();
        }
        //
        // public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        // {
        //     var user = await FindByIdAsync(id);
        //     user.Email = email;
        //     user.FirstName = firstName;
        //     user.LastName = lastName;
        //
        //     await _dbContext.SaveChangesAsync();
        // }
        //
        // public async Task MarkEmailAsVerifiedAsync(long id)
        // {
        //     var user = await FindByIdAsync(id);
        //     user.IsEmailVerified = true;
        //     user.LastRequest = DateTime.UtcNow;
        //
        //     await _dbContext.SaveChangesAsync();
        // }
        //

        //
        // public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        // {
        //     var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
        //         u.Id != userIdToExclude &&
        //         u.Email == email
        //     );
        //
        //     return user != null;
        // }

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
            var user = await _unitOfWork.Users.FindByEmail(payload.Email);
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
            
                _unitOfWork.Users.Add(user);
            }
            else if (!user.OAuthGoogle)
            {
                user.OAuthGoogle = true;
                user.LastRequest = DateTime.UtcNow;
            }

            var tokens = GenerateTokens(user.Id);
            _unitOfWork.Tokens.AddRange(tokens);

            await _unitOfWork.Complete();

            _authSqlService.SetTokens(tokens);
        }
    }
}
