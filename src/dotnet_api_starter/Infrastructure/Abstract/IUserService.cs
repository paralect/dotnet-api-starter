using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public interface IUserService
    {
        Task<bool> MarkEmailAsVerified(ObjectId _id);
        Task<bool> UpdateResetPasswordToken(ObjectId _id, string token);
        Task<bool> UpdatePassword(ObjectId _id, string newPassword);
        Task<bool> UpdateInfo(ObjectId _id, string email, string firstName, string lastName);
    }
}
