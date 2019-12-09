using Api.Core.Abstract;
using Api.Core.Utils;
using Api.Core.Models.User;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Api.Core.Services
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

        public async Task<bool> MarkEmailAsVerified(string id)
        {
            return await _userRepository.Update(id, x => new User { IsEmailVerified = true });
        }

        public async Task<bool> UpdateResetPasswordToken(string id, string token)
        {
            return await _userRepository.Update(id, x => new User { ResetPasswordToken = token });
        }

        public async Task<bool> UpdatePassword(string id, string newPassword)
        {
            string hash = newPassword.GetHash();

            return await _userRepository.Update(id, x => new User
            {
                PasswordHash = hash
            }); 
        }

        public async Task<bool> UpdateInfo(string id, string email, string firstName, string lastName)
        {
            return await _userRepository.Update(id, x => new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            });
        }

        public async Task<User> CreateUserAccount(string email, string firstName, string lastName, string password)
        {
            string hash = SecurityUtils.GetHash(password);
            string signupToken = SecurityUtils.GenerateSecureToken();

            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = hash,
                Email = email,
                IsEmailVerified = false,
                SignupToken = signupToken
            };

            _emailService.SendSignupWelcome(newUser);

            await _userRepository.Insert(newUser);

            return newUser;
        }

    }
}
