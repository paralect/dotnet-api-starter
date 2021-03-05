namespace Common.DAL.Documents.User
{
    public class UserFactory
    {
        public User UnverifiedUser()
        {
            var builder = new UserBuilder();
            return builder.NotVerifiedEmail().Build();
        }

        public User VerifiedUser()
        {
            var builder = new UserBuilder();
            return builder.Build();
        }
    }
}
