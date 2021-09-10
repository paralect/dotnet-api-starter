using System;
using Common.Enums;
using Common.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DB.Mongo.DAL.Documents.User
{
    public class User : BaseMongoDocument, IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [BsonIgnoreIfNull]
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public UserRoleEnum Role { get; set; }
        [BsonIgnoreIfNull]
        public string SignupToken { get; set; }
        public DateTime LastRequest { get; set; }
        [BsonElement("oauthGoogle")]
        public bool OAuthGoogle { get; set; }
        [BsonIgnoreIfNull]
        public string ResetPasswordToken { get; set; }
    }
}