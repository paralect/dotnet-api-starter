using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Services.Document.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Document;
using Api.Core.Services.Interfaces.Infrastructure;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using Common.Services;
using Common.Utils;
using LinqToDB;

namespace Api.Core.Services.Document
{
    public class UserService : BaseDocumentService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository,
            IEmailService emailService) : base(userRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            return _userRepository.GetQuery().FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task MarkEmailAsVerifiedAsync(long id)
        {
            return _userRepository.GetUpdateQuery(id).Set(x => x.IsEmailVerified, true).UpdateAsync();
        }

        public Task UpdateLastRequestAsync(long id)
        {
            return _userRepository.GetUpdateQuery(id).Set(u => u.LastRequest, DateTime.UtcNow).UpdateAsync();
        }

        public Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            return _userRepository.GetUpdateQuery(id).Set(u => u.ResetPasswordToken, token).UpdateAsync();
        }

        public Task UpdatePasswordAsync(long id, string newPassword)
        {
            return _userRepository.GetUpdateQuery(id)
                .Set(user => user.PasswordHash, newPassword.GetHash())
                .Set(user => user.ResetPasswordToken, string.Empty)
                .UpdateAsync();
        }

        public Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            return _userRepository.GetUpdateQuery(id)
               .Set(user => user.Email, email)
               .Set(user => user.FirstName, firstName)
               .Set(user => user.LastName, lastName)
               .UpdateAsync();
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
                SignupToken = signUpToken
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
                OAuthGoogle = true
            };

            await _userRepository.InsertAsync(newUser);

            return newUser;
        }

        public Task EnableGoogleAuthAsync(long id)
        {
            return _userRepository.GetUpdateQuery(id)
               .Set(user => user.OAuthGoogle, true)
               .UpdateAsync();
        }

        public async Task<bool> IsEmailInUseAsync(long? userIdToExclude, string email)
        {
            var usersQuery = _userRepository.GetQuery().Where(x => x.Email.ToUpper() == email.ToUpper());
            if (userIdToExclude.HasValue)
            {
                usersQuery = usersQuery.Where(x => x.Id != userIdToExclude);
            }

            return !await usersQuery.AnyAsync();
        }

        public async Task<long?> FindUserIDByResetPasswordTokenAsync(string resetPasswordToken)
        {
            return (await _userRepository.GetQuery()
                 .Where(x => x.ResetPasswordToken == resetPasswordToken)
                 .Select(x => new { x.Id })
                 .FirstOrDefaultAsync()).Id;
        }

        public async Task<long?> FindUserIDBySignUpTokenAsync(string signUpToken)
        {
            return (await _userRepository.GetQuery()
                 .Where(x => x.SignupToken == signUpToken)
                 .Select(x => new { x.Id })
                 .FirstOrDefaultAsync()).Id;
        }
    }
}
