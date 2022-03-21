using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using IIdGenerator = Common.Dal.Interfaces.IIdGenerator;
using ValidationAttribute = Api.NoSql.Security.ValidationAttribute;
using Common.Dal.Interfaces;
using Common.Dal;
using Api.NoSql.Services.Interfaces;
using Api.NoSql.Utils;
using Api.NoSql.Mapping;
using System.Collections.Generic;
using System;
using Common.Services.Domain.Interfaces;

namespace Api.NoSql
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

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);
            ConfigureDi(services);
            ConfigureDb(services);

            services.AddHttpContextAccessor();

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

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseTokenAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<DbSettings>(options => { _configuration.GetSection("Db").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
        }

        private void ConfigureDi(IServiceCollection services)
        {
            // replace with simpler version, if SQL DAL is removed from the solution:
            // services.AddTransientByConvention(
            //     typeof(IRepository<,>),
            //     t => t.Name.EndsWith("Repository")
            // );

            // services.AddTransientByConvention(
            //     new List<Type> { typeof(IAuthService), typeof(IUserService) },
            //     t => t.Name.EndsWith("Service")
            // );

            services.AddTransientByConvention(
                new List<Type> { typeof(IRepository<,>) },
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository"),
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository")
            );

            // register services from Api project
            services.AddTransientByConvention(
                typeof(IAuthService),
                t => t.Name.EndsWith("Service")
            );

            // register services from Common project
            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                t => t.Namespace.StartsWith("Common.Services.") && t.Name.EndsWith("Service"),
                t => t.Namespace.StartsWith("Common.Services.") && t.Name.EndsWith("Service")
            );

            services.AddTransient<IDbContext, DbContext>();

            services.AddTransient<IIdGenerator, IdGenerator>();
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var dbSettings = new DbSettings();
            _configuration.GetSection("Db").Bind(dbSettings);

            services.InitializeDb(dbSettings);
        }
    }
}
