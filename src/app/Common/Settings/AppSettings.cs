namespace Common.Settings
{
    public class AppSettings
    {
        public string WebUrl { get; set; }
        public string LandingUrl { get; set; }
        public AuthorizationDatabaseEnum AuthorizationDatabase { get; set; }
    }

    public enum AuthorizationDatabaseEnum
    {
        Mongo = 1,
        Postgres = 2
    }
}
