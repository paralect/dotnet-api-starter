using System;
using System.Collections.Generic;

namespace Common.DALSql.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string SignupToken { get; set; }
        public DateTime LastRequest { get; set; }
        public string ResetPasswordToken { get; set; }
        public bool OAuthGoogle { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }
}
