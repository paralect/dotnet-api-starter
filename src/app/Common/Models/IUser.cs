using System;

namespace Common.Models
{
    public interface IUser : IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string SignupToken { get; set; }
        public DateTime LastRequest { get; set; }
        public bool OAuthGoogle { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
