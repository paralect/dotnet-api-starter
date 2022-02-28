using Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DALSql
{
    public static class PostgreSqlDbInitializer
    {
        public static void InitializeDb(this IServiceCollection services, DbSettingsSql settings)
        {
            services.AddDbContext<ShipDbContext>(options =>
                options.UseNpgsql(settings.ConnectionString));
        }
    }
}