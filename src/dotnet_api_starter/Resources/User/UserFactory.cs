using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Resources.User
{
    public class UserFactory
    {
        public UserDocument UnverifiedUser()
        {
            UserBuilder builder = new UserBuilder();
            return builder.NotVerifiedEmail().Build();
        }

        public UserDocument VerifiedUser()
        {
            UserBuilder builder = new UserBuilder();
            return builder.Build();
        }
    }
}
