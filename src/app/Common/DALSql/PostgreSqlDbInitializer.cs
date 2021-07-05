using Common.DALSql.Data;
using Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// TODO rename namespace
namespace Common.DALSql
{
    public static class PostgreSqlDbInitializer
    {
        public static void InitializePostgreSqlDb(this IServiceCollection services, SqlConnectionSettings settings)
        {
            services.AddDbContext<ShipContext>(options =>
                options.UseNpgsql(settings.ConnectionString));
        }
    }
}
