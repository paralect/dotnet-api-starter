using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Resources.User
{
    public class UserDocument
    {
        [BsonId]
        public ObjectId _id { get; set; }
                
        public DateTime CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string SignupToken { get; set; }
        public string ResetPasswordToken { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
