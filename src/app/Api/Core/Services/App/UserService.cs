using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Services.App.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Utils;

namespace Api.Core.Services.App
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _userRepository.FindByIdAsync(id);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userRepository.FindOneAsync(new UserFilter {Email = email});
        }

        public async Task<User> FindBySignupTokenAsync(string signupToken)
        {
            return await _userRepository.FindOneAsync(new UserFilter {SignupToken = signupToken});
        }

        public async Task<User> FindByResetPasswordTokenAsync(string resetPasswordToken)
        {
            return await _userRepository.FindOneAsync(new UserFilter {ResetPasswordToken = resetPasswordToken});
        }

        public async Task MarkEmailAsVerifiedAsync(string id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.IsEmailVerified,  true);
        }

        public async Task UpdateLastRequestAsync(string id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.LastRequest, DateTime.UtcNow);
        }

        public async Task UpdateResetPasswordTokenAsync(string id, string token)
        {
            await _userRepository.UpdateOneAsync(id, u => u.ResetPasswordToken, token);
        }

        public async Task UpdatePasswordAsync(string id, string newPassword)
        {
            await _userRepository.UpdateOneAsync(id, new Dictionary<Expression<Func<User, object>>, object>
            {
                {u => u.PasswordHash, newPassword.GetHash()},
                {u => u.ResetPasswordToken, string.Empty}
            });
        }

        public async Task UpdateInfoAsync(string id, string email, string firstName, string lastName)
        {
            await _userRepository.UpdateOneAsync(id, new Dictionary<Expression<Func<User, object>>, object>
            {
                {u => u.Email, email},
                {u => u.FirstName, firstName},
                {u => u.LastName, lastName}
            });
        }

        public async Task<User> CreateUserAccountAsync(CreateUserModel model)
        {
            var signupToken = SecurityUtils.GenerateSecureToken();

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signupToken,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            await _userRepository.InsertAsync(newUser);

            _emailService.SendSignupWelcome(new SignupWelcomeModel
            {
                Email = model.Email,
                SignupToken = signupToken
            });

            return newUser;
        }

        public async Task<User> CreateUserAccountAsync(CreateUserGoogleModel model)
        {
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsEmailVerified = true,
                OAuth = new User.OAuthSettings
                {
                    Google = true
                }
            };

            await _userRepository.InsertAsync(newUser);

            return newUser;
        }

        public async Task EnableGoogleAuthAsync(string id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.OAuth.Google, true);
        }

        public async Task<bool> IsEmailInUseAsync(string userIdToExclude, string email)
        {
            var user = await _userRepository
                .FindOneAsync(new UserFilter {UserIdToExclude = userIdToExclude, Email = email});

            return user != null;
        }
    }
}
