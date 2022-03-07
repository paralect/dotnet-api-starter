using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Services.Document.Models;
using Api.Services.Infrastructure;
using Api.Services.Infrastructure.Models;
using Common.DAL;
using Common.DAL.Documents.User;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.DAL.UpdateDocumentOperators;
using Common.Utils;

namespace Api.Services.Document
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

        public async Task<Page<User>> FindPageAsync(
            UserFilter filter,
            IList<(string, SortDirection)> sortFields,
            int page,
            int pageSize
        )
        {
            return await _userRepository.FindPageAsync(filter, sortFields, page, pageSize);
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
            await _userRepository.UpdateOneAsync(id, new IUpdateOperator<User>[]
            {
                new SetOperator<User, string>(user => user.PasswordHash, newPassword.GetHash()),
                new SetOperator<User, string>(user => user.ResetPasswordToken, string.Empty)
            });
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
                Role = Common.Enums.UserRole.User
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
