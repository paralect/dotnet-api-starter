using Api.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Models.User
{
    public class UserBuilder : BaseBuilder<User>
    {
        public UserBuilder()
        {
            data.CreatedOn = DateTime.Now;
            data.FirstName = "first_name";
            data.LastName = "last_name";
            data.SignupToken = null;
            data.ResetPasswordToken = "reset_password_token";
            data.IsEmailVerified = true;

            Email();
            Password();
            SignupToken();
        }

        public UserBuilder Email(string email = "e@ma.il")
        {
            data.Email = email;
            return this;
        }

        public UserBuilder Password(string passwordHash = "hash", string passwordSalt = "salt")
        {
            data.PasswordHash = passwordHash;
            data.PasswordSalt = passwordSalt;
            return this;
        }

        public UserBuilder NotVerifiedEmail()
        {
            data.IsEmailVerified = false;
            return this;
        }

        public UserBuilder SignupToken(string token = "token")
        {
            data.SignupToken = token;
            return this;
        }
    }
}
