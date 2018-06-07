using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Infrastructure.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Resources.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> MarkEmailAsVerified(ObjectId _id)
        {
            return await _userRepository.UpdateField(_id, x => x.IsEmailVerified, true);
        }

        public async Task<bool> UpdateResetPasswordToken(ObjectId _id, string token)
        {
            return await _userRepository.UpdateField(_id, x => x.ResetPasswordToken, token);
        }

        public async Task<bool> UpdatePassword(ObjectId _id, string newPassword)
        {
            string salt = SecurityUtils.GenerateSalt();
            string hash = newPassword.GetHash(salt);

            UserDocument user = _userRepository.FindOne(x => x._id == _id);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            return await _userRepository.Replace(user);
        }

        public async Task<bool> UpdateInfo(ObjectId _id, string email, string firstName, string lastName)
        {
            UserDocument user = _userRepository.FindOne(x => x._id == _id);

            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            return await _userRepository.Replace(user);
        }
    }
}
