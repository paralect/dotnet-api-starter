using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.DAL.Documents.User
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
        public bool IsEmailVerified { get; set; }
        [BsonIgnoreIfNull]
        public string SignupToken { get; set; }
        [BsonElement("oauth")]
        public OAuthSettings OAuth { get; set; }
        public DateTime LastRequest { get; set; }
        [BsonIgnoreIfNull]
        public string ResetPasswordToken { get; set; }
        
        public class OAuthSettings
        {
            public bool Google { get; set; }
        }
    }
}