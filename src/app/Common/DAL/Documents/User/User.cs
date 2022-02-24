using System;
using Common.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.DAL.Documents.User
{
    public class User : BaseDocument
    {
        public User()
        {
            OAuth = new OAuthSettings();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        [BsonIgnoreIfNull]
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public UserRoleEnum Role { get; set; }
        public bool IsEmailVerified { get; set; }
        [BsonIgnoreIfNull]
        public string SignupToken { get; set; }
        [BsonIgnoreIfNull]
        public string ResetPasswordToken { get; set; }
        [BsonElement("oauth")]
        public OAuthSettings OAuth { get; set; }
        public DateTime LastRequest { get; set; }


        public class OAuthSettings
        {
            public bool Google { get; set; }
        }
    }
}