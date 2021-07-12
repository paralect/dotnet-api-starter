using System;
using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Services.Domain
{
    public class UserSqlService : IUserSqlService
    {
        private readonly ShipDbContext _dbContext;
        private readonly IEmailService _emailService;

        public UserSqlService(ShipDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task<User> FindByIdAsync(long id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id); // TODO consider AsNoTracking
        }

        public async Task<User> FindBySignupTokenAsync(string token)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.SignupToken == token);
        }

        public async Task<User> FindByResetPasswordTokenAsync(string token)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }

        public async Task<User> CreateUserAccountAsync(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var user = _dbContext.Users.Add(new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken
            }).Entity;

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> CreateUserAccountAsync(CreateUserGoogleModel model)
        {
            var user = _dbContext.Users.Add(new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsEmailVerified = true,
                OAuthGoogle = true
            }).Entity;

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateLastRequestAsync(long id)
        {
            var user = await FindByIdAsync(id);
            user.LastRequest = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await FindByIdAsync(id);
            user.ResetPasswordToken = token;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await FindByIdAsync(id);
            user.PasswordHash = newPassword.GetHash();
            user.ResetPasswordToken = string.Empty;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            var user = await FindByIdAsync(id);
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkEmailAsVerifiedAsync(long id)
        {
            var user = await FindByIdAsync(id);
            user.IsEmailVerified = true;
            user.LastRequest = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task EnableGoogleAuthAsync(long id)
        {
            var user = await FindByIdAsync(id);
            user.OAuthGoogle = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Id != userIdToExclude &&
                u.Email == email
            );

            return user != null;
        }
    }
}
