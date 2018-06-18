using Api.Core.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Models.User
{
    public class User : BaseModel
    {
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