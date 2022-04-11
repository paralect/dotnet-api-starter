using Common.Jobs;
using Common.Settings;
using Common.Utils;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
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

var recurringJobManager = host.Services.GetRequiredService<IRecurringJobManager>();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var schedulerSettings = configuration.GetSection("Scheduler").Get<SchedulerSettings>();
scheduleRecurringJobs(schedulerSettings.Jobs);

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
            var dbSettings = context.Configuration.GetSection("Db").Get<DbSettings>();

            services.AddHangfire(config =>
            {
                config.UseMongoStorage(dbSettings.ConnectionStrings.Scheduler, new MongoStorageOptions
                {
                    // https://github.com/sergeyzwezdin/Hangfire.Mongo#migration
                    // be careful with migration strategy when updating Hangfire.Mongo package
                    // Note: migration from first to the last version across a few running instances of Scheduler and API can throw exception
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection // TODO set to Watch for envs with replica sets
                });
            });

            services.AddTransientByConvention(
                new List<Type> { typeof(IHelloWorldJob), typeof(HelloWorldJob) },
                t => t.Name.EndsWith("Job") && t != typeof(ISchedulerJob));
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

void scheduleRecurringJobs(Jobs jobs)
{
    scheduleRecurringJob<IHelloWorldJob>(jobs.HelloWorld);
}

void scheduleRecurringJob<T>(BaseJobConfig jobConfig)
    where T : ISchedulerRecurringJob
{
    recurringJobManager.AddOrUpdate<T>(jobConfig.Name, j => j.Execute(), jobConfig.Schedule);
}