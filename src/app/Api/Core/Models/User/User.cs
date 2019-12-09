using Api.Core.Abstract;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Core.Models.User
{
    public class User : BaseModel
    {
        public User()
        {
            OAuth = new OAuthSettings();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string SignupToken { get; set; }
        public string ResetPasswordToken { get; set; }
        [BsonElement("oauth")]
        public OAuthSettings OAuth { get; set; }

        public class OAuthSettings
        {
            public bool Google { get; set; }
        }
    }
}