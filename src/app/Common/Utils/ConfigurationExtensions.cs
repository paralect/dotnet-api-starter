using Common.Settings;
using Microsoft.Extensions.Configuration;

namespace Common.Utils;

public static class ConfigurationExtensions
{
    public static AppSettings GetAppSettings(this IConfiguration configuration) =>
        configuration.GetSection("App").Get<AppSettings>();
    
    public static DbSettings GetDbSettings(this IConfiguration configuration) =>
        configuration.GetSection("Db").Get<DbSettings>();
}