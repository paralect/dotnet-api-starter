using System;
using System.Threading.Tasks;
using Api.Core.DbViews.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
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

        public User FindByEmail(string email)
        {
            return _userRepository.FindOne(x => x.Email == email);
        }

        public User FindBySignupToken(string signupToken)
        {
            return _userRepository.FindOne(x => x.SignupToken == signupToken);
        }

        public User FindByResetPasswordToken(string resetPasswordToken)
        {
            return _userRepository.FindOne(x => x.ResetPasswordToken == resetPasswordToken);
        }

        public async Task MarkEmailAsVerified(string id)
        {
            await _userRepository.Update(id, x => new User { IsEmailVerified = true });
        }

        public async Task UpdateLastRequest(string id)
        {
            await _userRepository.Update(id, x => new User { LastRequest = DateTime.UtcNow });
        }

        public async Task UpdateResetPasswordToken(string id, string token)
        {
            await _userRepository.Update(id, x => new User { ResetPasswordToken = token });
        }

        public async Task UpdatePassword(string id, string newPassword)
        {
            var hash = newPassword.GetHash();

            await _userRepository.Update(id, x => new User
            {
                PasswordHash = hash,
                ResetPasswordToken = string.Empty
            }); 
        }

        public async Task UpdateInfo(string id, string email, string firstName, string lastName)
        {
            await _userRepository.Update(id, x => new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            });
        }

        public async Task<User> CreateUserAccount(string email, string firstName, string lastName, string password)
        {
            var hash = password.GetHash();
            var signupToken = SecurityUtils.GenerateSecureToken();

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = hash,
                Email = email,
                IsEmailVerified = false,
                SignupToken = signupToken,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            _emailService.SendSignupWelcome(new SignupWelcomeModel
            {
                Email = email,
                SignupToken = signupToken
            });

            await _userRepository.Insert(newUser);

            return newUser;
        }
    }
}
