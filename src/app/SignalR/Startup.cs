using Common.Dal.Interfaces;
using Common.Dal;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalR.Hubs;
using SignalR.Mapping;
using SignalR.Services;
using System.Collections.Generic;
using System;
using Common.Services.NoSql.Domain.Interfaces;
using Common;
using Serilog;

namespace SignalR
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDi(services);
            ConfigureDb(services);
            ConfigureCors(services);

            services.AddSignalR();
            services.AddHostedService<ChangeStreamBackgroundService>();
            services.AddAutoMapper(typeof(UserProfile));
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseTokenAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UserHub>("");
                endpoints.MapHealthChecks(Constants.HealthcheckPath);
            });
        }

        private void ConfigureDi(IServiceCollection services)
        {
            // replace with simpler version, if SQL DAL is removed from the solution:
            // services.AddTransientByConvention(
            //     typeof(IRepository<,>),
            //     t => t.Name.EndsWith("Repository")
            // );

            // services.AddTransientByConvention(
            //     typeof(IUserService),
            //     t => t.Name.EndsWith("Service")
            // );

            services.AddTransientByConvention(
                new List<Type> { typeof(IRepository<,>) },
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository"),
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository")
            );

            Predicate<Type> predicate = t =>
                (
                    t.Namespace.StartsWith("Common.Services.NoSql.") ||
                    t.Namespace.StartsWith("Common.Services.Infrastructure.")
                )
                && t.Name.EndsWith("Service");

            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                predicate,
                predicate
            );

            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<IIdGenerator, IdGenerator>();

            services.AddTransient<IUserHubContext, UserHubContext>();

            services.Configure<DbSettings>(options => { _configuration.GetSection("Db").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var dbSettings = new DbSettings();
            _configuration.GetSection("Db").Bind(dbSettings);

            services.InitializeDb(dbSettings);
        }

        private void ConfigureCors(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            _configuration.GetSection("App").Bind(appSettings);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder
                        .WithOrigins(appSettings.WebUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}
