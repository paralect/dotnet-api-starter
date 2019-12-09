namespace Api.Core.Settings
{
    public class AppSettings
    {
        public string WebUrl { get; set; }
        public string LandingUrl { get; set; }
        public int AccessTokenExpiresInHours { get; set; }
        public int RefreshTokenExpiresInHours { get; set; }
    }
}
