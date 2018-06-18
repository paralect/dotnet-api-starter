using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Models.User
{
    public class UserFactory
    {
        public User UnverifiedUser()
        {
            UserBuilder builder = new UserBuilder();
            return builder.NotVerifiedEmail().Build();
        }

        public User VerifiedUser()
        {
            UserBuilder builder = new UserBuilder();
            return builder.Build();
        }
    }
}
