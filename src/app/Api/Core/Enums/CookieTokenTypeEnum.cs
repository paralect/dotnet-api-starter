using System.ComponentModel;

namespace Api.Core.Enums
{
    public enum CookieTokenTypeEnum
    {
        [Description("access_token")]
        AccessToken,
        [Description("refresh_token")]
        RefreshToken
    }
}
