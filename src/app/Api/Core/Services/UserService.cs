using Api.Core.Abstract;
using Api.Core.Utils;
using Api.Core.Models.User;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> MarkEmailAsVerified(ObjectId id)
        {
            return await _userRepository.Update(id, x => new User { IsEmailVerified = true });
        }

        public async Task<bool> UpdateResetPasswordToken(ObjectId id, string token)
        {
            return await _userRepository.Update(id, x => new User { ResetPasswordToken = token });
        }

        public async Task<bool> UpdatePassword(ObjectId id, string newPassword)
        {
            string salt = SecurityUtils.GenerateSalt();
            string hash = newPassword.GetHash(salt);

            return await _userRepository.Update(id, x => new User
            {
                PasswordHash = hash,
                PasswordSalt = salt
            }); 
        }

        public async Task<bool> UpdateInfo(ObjectId id, string email, string firstName, string lastName)
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
            string salt = SecurityUtils.GenerateSalt();
            string hash = SecurityUtils.GetHash(password, salt);
            string signupToken = SecurityUtils.GenerateSecureToken();

            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = hash,
                PasswordSalt = salt,
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
