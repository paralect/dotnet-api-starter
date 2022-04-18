using Api.NoSql;
using Api.Views.Mappings;
using Api.Views.Validators.Account;
using Common;
using Common.Dal;
using Common.Settings;
using Common.Utils;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

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

services.ConfigureDi();
services.ConfigureCache(cacheSettings);
services.ConfigureCors(appSettings);
services.ConfigureControllers();
services.ConfigureSwagger();

services.AddHttpContextAccessor();
services.AddAuthorization();
services.AddAutoMapper(typeof(UserProfile));
services.AddFluentValidation(config =>
    config.RegisterValidatorsFromAssemblyContaining(typeof(SignInModelValidator))
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint(Constants.Swagger.Url, Constants.Swagger.Name)
);

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
    endpoints.MapControllers();
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