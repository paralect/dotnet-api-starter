using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.DbViews.User
{
    public class User : BaseView
    {
        public User()
        {
            OAuth = new OAuthSettings();
            ResetPasswordToken = string.Empty;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string SignupToken { get; set; }
        [BsonElement("oauth")]
        public OAuthSettings OAuth { get; set; }
        public DateTime LastRequest { get; set; }
        public string ResetPasswordToken { get; set; }
        
        public class OAuthSettings
        {
            public bool Google { get; set; }
        }
    }
}