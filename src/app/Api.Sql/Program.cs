using Common.DalSql;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Sql
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
            Log.Logger = hostEnvironment.BuildLogger();

            try
            {
                Log.Information("Starting migrations");
                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ShipDbContext>();
                    db.Database.Migrate();
                }

                Log.Information("Starting web host");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
