namespace Api.Core.DAL.Views.User
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
