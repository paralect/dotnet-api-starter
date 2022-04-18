using Api.Views.Mappings;
using Common;
using Common.Dal;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SignalR.Hubs;
using SignalR.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;

builder.Host.UseSerilog();
Log.Logger = environment.BuildLogger();

var dbSettings = services.ConfigureSettings<DbSettings>(configuration, "Db");
var appSettings = services.ConfigureSettings<AppSettings>(configuration, "App");
var cacheSettings = services.ConfigureSettings<CacheSettings>(configuration, "Cache");
services.ConfigureSettings<TokenExpirationSettings>(configuration, "TokenExpiration");
services.ConfigureSettings<EmailSettings>(configuration, "Email");

services.InitializeDb(dbSettings);

//services.ConfigureDi(services);
services.ConfigureCache(cacheSettings);
services.ConfigureCors(appSettings);

services.AddHttpContextAccessor();
services.AddSignalR();
services.AddHostedService<ChangeStreamBackgroundService>();
services.AddAutoMapper(typeof(UserProfile));

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors(Constants.CorsPolicy.AllowSpecificOrigin);
app.UseTokenAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<UserHub>(string.Empty);
    endpoints.MapHealthChecks(Constants.HealthcheckPath, new HealthCheckOptions
    {
        AllowCachingResponses = false
    });
});

try
{
    Log.Information("Starting web host");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}