using MongoDB.Driver;

namespace Common
{
    public static class Constants
    {
        public class DbDocuments
        {
            public const string Users = "users";
            public const string Tokens = "tokens";
        }

        public const int TokenSecurityLength = 32;

        public class CookieNames
        {
            public const string AccessToken = "access_token";
            public const string RefreshToken = "refresh_token";
        }

        public static readonly Collation DefaultCollation = new Collation("en", strength: CollationStrength.Primary);
    }
}
