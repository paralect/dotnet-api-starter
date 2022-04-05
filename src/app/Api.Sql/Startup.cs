using System;
using System.Collections.Generic;
using Api.Sql.Mapping;
using Common;
using Common.DalSql;
using Common.DalSql.Interfaces;
using Common.Services.Sql.Domain.Interfaces;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using ValidationAttribute = Common.Security.ValidationAttribute;

namespace Api.Sql
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);
            ConfigureDi(services);
            ConfigureDb(services);
            ConfigureCors(services);

            services.AddHttpContextAccessor();

            services
                .AddControllers(o => o.Filters.Add(typeof(ValidationAttribute)))
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.GetErrors();
                        var result = new BadRequestObjectResult(errors);

                        return result;
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAuthorization();

            services.AddAutoMapper(typeof(UserProfile));

            services
                .AddHealthChecks()
                .AddDbContextCheck<ShipDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseTokenAuthenticationSql();

            // The middleware makes requests to DB, if there are any changes on EF DbContext.
            // It's still possible to update DB manually from controllers/services -
            // in this case the middleware does nothing
            app.UseDbContextSaveChanges();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(Constants.HealthcheckPath, new HealthCheckOptions
                {
                    AllowCachingResponses = false
                });
            });
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<DbSettingsSql>(options => { _configuration.GetSection("DbSql").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
        }

        private void ConfigureDi(IServiceCollection services)
        {
            // replace with simpler version, if MongoDB DAL is removed from the solution:
            // services.AddTransientByConvention(
            //     typeof(IRepository<,>),
            //     t => t.Name.EndsWith("Repository")
            // );

            // services.AddTransientByConvention(
            //      new List<Type> { typeof(IAuthService), typeof(IUserService) },
            //     t => t.Name.EndsWith("Service")
            // );

            services.AddTransientByConvention(
                new List<Type> { typeof(IRepository<,>) },
                t => t.Namespace.StartsWith("Common.DalSql.") && t.Name.EndsWith("Repository"),
                t => t.Namespace.StartsWith("Common.DalSql.") && t.Name.EndsWith("Repository")
            );

            Predicate<Type> predicate = t =>
                (
                    t.Namespace.StartsWith("Common.Services.Sql.") ||
                    t.Namespace.StartsWith("Common.Services.Infrastructure.")
                )
                && t.Name.EndsWith("Service");

            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                predicate,
                predicate
            );

            // register services from Common.Services project
            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                predicate,
                predicate
            );
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var dbSettings = new DbSettingsSql();
            _configuration.GetSection("DbSql").Bind(dbSettings);

            services.InitializeDb(dbSettings);
        }

        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    var appSettings = new AppSettings();
                    _configuration.GetSection("App").Bind(appSettings);

                    builder
                        .WithOrigins(appSettings.LandingUrl, appSettings.WebUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}
