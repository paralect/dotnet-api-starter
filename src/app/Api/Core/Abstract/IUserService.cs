using Api.Core.Models.User;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IUserService
    {
        Task<bool> MarkEmailAsVerified(ObjectId id);

        Task<bool> UpdateResetPasswordToken(ObjectId id, string token);

        Task<bool> UpdatePassword(ObjectId id, string newPassword);

        Task<bool> UpdateInfo(ObjectId id, string email, string firstName, string lastName);

        Task<User> CreateUserAccount(string email, string firstName, string lastName, string password);
    }
}
