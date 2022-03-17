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
using Common.Services.Domain.Interfaces;
using System.Collections.Generic;
using System;

namespace SignalR
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddJsonFile($"common.{env.EnvironmentName}.json");

            _configuration = builder.Build();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowSpecificOrigin");
            app.UseRouting();
            app.UseTokenAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UserHub>("");
            });
        }

        private void ConfigureDi(IServiceCollection services)
        {
            // replace with simpler version, if MongoDB DAL is removed from the solution:
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
                t => t.Namespace.StartsWith("Common.Dal."),
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository")
            );

            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                t => t.Namespace.StartsWith("Common.Dal."),
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Service")
            );

            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<IIdGenerator, IdGenerator>();

            services.AddTransient<IUserHubContext, UserHubContext>();

            services.Configure<DbSettings>(options => { _configuration.GetSection("DB").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var dbSettings = new DbSettings();
            _configuration.GetSection("DB").Bind(dbSettings);

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
