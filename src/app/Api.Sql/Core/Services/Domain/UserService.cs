using System;
using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Common.Utils;

namespace Api.Core.Services.Domain
{
    public class UserService : IUserService
    {
        private readonly ShipDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;

        public UserService(
            ShipDbContext dbContext,
            IEmailService emailService,
            IAuthService authSqlService)
        {
            _dbContext = dbContext;

            _emailService = emailService;
            _authService = authSqlService;
        }

        public User CreateUserAccount(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken,
                CreatedOn = DateTime.UtcNow // TODO do in repository
            };

            _dbContext.Users.Add(user);

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return user;
        }

        public async Task VerifyEmailAsync(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            user.IsEmailVerified = true;
            user.LastRequest = DateTime.UtcNow;

            _authService.SetTokens(userId);
        }

        public async Task SignInAsync(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            user.LastRequest = DateTime.UtcNow;
            _authService.SetTokens(userId);
        }

        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.ResetPasswordToken = token;
        }

        public async Task<string> SetResetPasswordTokenAsync(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user.ResetPasswordToken.HasNoValue())
            {
                user.ResetPasswordToken = SecurityUtils.GenerateSecureToken();
            }

            return user.ResetPasswordToken;
        }

        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.PasswordHash = newPassword.GetHash();
            user.ResetPasswordToken = string.Empty;
        }

        public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
        }

        public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                IdToExclude = userIdToExclude,
                Email = email,
                AsNoTracking = true
            });

            return user != null;
        }

        public async Task SignInGoogleWithCodeAsync(GooglePayloadModel payload)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Email = payload.Email
            }
                .Include(u => u.Tokens)
            );
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

                _dbContext.Users.Add(user);
            }
            else
            {
                if (!user.OAuthGoogle)
                {
                    user.OAuthGoogle = true;
                    user.LastRequest = DateTime.UtcNow;
                }
            }

            _authService.SetTokens(user);
        }
    }
}