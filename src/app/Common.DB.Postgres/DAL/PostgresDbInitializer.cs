using System;
using Common.DB.Postgres.Settings;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DB.Postgres.DAL
{
    public static class PostgresDbInitializer
    {
        public static void InitializeDb(IServiceCollection services, PostgresDbSettings dbSettings)
        {
            var serviceProvider = CreateServices(services, dbSettings);

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using var scope = serviceProvider.CreateScope();
            UpdateDatabase(scope.ServiceProvider);
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices(IServiceCollection services, PostgresDbSettings dbSettings)
        {
            return services
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(dbSettings.ConnectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migrations.Init).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }
    }
}
