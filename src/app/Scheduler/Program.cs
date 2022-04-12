using Common.Jobs;
using Common.Settings;
using Common.Utils;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scheduler.Jobs;
using Scheduler.Settings;
using Scheduler.Settings.JobConfigs;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using var host = createHostBuilder(args).Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Log.Logger = buildLogger();

scheduleRecurringJobs();

try
{
    Log.Information("Starting host");
    using (new BackgroundJobServer())
    {
        Log.Information("Hangfire Server started. Press any key to exit...");
        await host.RunAsync();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

IHostBuilder createHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            var dbSettings = context.Configuration.GetDbSettings();
            var schedulerSettings = context.Configuration.GetSection("Scheduler").Get<SchedulerSettings>();

            services.AddHangfire(config =>
            {
                switch (schedulerSettings.Storage)
                {
                    case HangfireStorage.MongoDb:
                        config.UseMongoStorage(dbSettings.ConnectionStrings.Scheduler, new MongoStorageOptions
                        {
                            MigrationOptions = new MongoMigrationOptions
                            {
                                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                BackupStrategy = new CollectionMongoBackupStrategy()
                            },
                            CheckConnection = true,
                            // set to Watch for envs with replica sets
                            CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                        });
                        break;
                    case HangfireStorage.PostgreSql:
                        config.UsePostgreSqlStorage(dbSettings.ConnectionStrings.Scheduler);
                        break;
                    default:
                        throw new InvalidOperationException(nameof(schedulerSettings.Storage));
                }
            });

            services.AddTransientByConvention(
                new List<Type> { typeof(IHelloWorldJob), typeof(HelloWorldJob) },
                t => t.Name.EndsWith("Job") && t != typeof(ISchedulerRecurringJob));
        });

ILogger buildLogger()
{
    var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Hangfire", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Logger(lc =>
        {
            if (hostEnvironment.IsDevelopment())
            {
                lc.WriteTo.Console();
            }
            else
            {
                lc.WriteTo.Console(new RenderedCompactJsonFormatter());
            }
        });

    return loggerConfig.CreateLogger();
}

void scheduleRecurringJobs()
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    var jobs = configuration.GetSection("Scheduler").Get<SchedulerSettings>().Jobs;
    scheduleRecurringJob<IHelloWorldJob>(jobs.HelloWorld);
}

void scheduleRecurringJob<T>(BaseJobConfig jobConfig)
    where T : ISchedulerRecurringJob
{
    var recurringJobManager = host.Services.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<T>(jobConfig.Name, j => j.ExecuteAsync(), jobConfig.Schedule);
}