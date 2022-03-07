using System;
using System.Threading.Tasks;
using Api.Core.Services.Document.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Document;
using Api.Core.Services.Interfaces.Infrastructure;
using Common.DAL.Documents.User;
using Common.DAL.FluentUpdater;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.Services;
using Common.Utils;

namespace Api.Core.Services.Document
{
    public class UserService : BaseDocumentService<User, UserFilter>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository,
            IEmailService emailService) : base(userRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await FindOneAsync(new UserFilter { Email = email });
        }

        public async Task MarkEmailAsVerifiedAsync(string id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.IsEmailVerified, true);
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
            await _userRepository.UpdateOneAsync(id, Updater<User>
                .Set(u => u.PasswordHash, newPassword.GetHash())
                .Set(u => u.ResetPasswordToken, string.Empty));
        }

        public async Task UpdateInfoAsync(string id, string email, string firstName, string lastName)
        {
            await _userRepository.UpdateOneAsync(id, Updater<User>
                .Set(u => u.Email, email)
                .Set(u => u.FirstName, firstName)
                .Set(u => u.LastName, lastName));
        }

        public async Task<User> CreateUserAccountAsync(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken,
                Role = Common.Enums.UserRoleEnum.User
            };

            await _userRepository.InsertAsync(newUser);

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
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
                .FindOneAsync(new UserFilter { UserIdToExclude = userIdToExclude, Email = email });

            return user != null;
        }
    }
}
