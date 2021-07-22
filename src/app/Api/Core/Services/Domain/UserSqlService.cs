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
    public class UserSqlService : IUserSqlService
    {
        private readonly ShipDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IAuthSqlService _authSqlService;

        public UserSqlService(
            ShipDbContext dbContext,
            IEmailService emailService,
            IAuthSqlService authSqlService)
        {
            _dbContext = dbContext;
            
            _emailService = emailService;
            _authSqlService = authSqlService;
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
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return user;
        }

        public async Task VerifyEmail(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            user.IsEmailVerified = true;
            user.LastRequest = DateTime.UtcNow;

            _authSqlService.SetTokens(userId);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SignIn(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            user.LastRequest = DateTime.UtcNow;
            _authSqlService.SetTokens(userId);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.ResetPasswordToken = token;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> SetResetPasswordToken(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user.ResetPasswordToken.HasNoValue())
            {
                user.ResetPasswordToken = SecurityUtils.GenerateSecureToken();
                await _dbContext.SaveChangesAsync();
            }

            return user.ResetPasswordToken;
        }

        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.PasswordHash = newPassword.GetHash();
            user.ResetPasswordToken = string.Empty;

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        {
            var user = await _dbContext.Users.FindOneByFilterAsNoTracking(new UserFilter
            {
                IdToExclude = userIdToExclude,
                Email = email
            });
        
            return user != null;
        }

        public async Task SignInGoogleWithCodeAsync(GooglePayloadModel payload)
        {
            var user = await _dbContext.Users.FindOneByFilterAsNoTracking(new UserFilter
            {
                Email = payload.Email
            });
            
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
                _dbContext.Attach(user);
                    
                if (!user.OAuthGoogle)
                {
                    user.OAuthGoogle = true;
                    user.LastRequest = DateTime.UtcNow;
                }
            }
                
            await _authSqlService.SetTokens(user);

            await _dbContext.SaveChangesAsync();
        }
    }
}
