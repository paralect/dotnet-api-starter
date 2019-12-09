using System.ComponentModel;

namespace Api.Core.Enums
{
    public enum TokenTypeEnum
    {
        [Description("refresh")]
        Refresh,
        [Description("access")]
        Access
    }
}
