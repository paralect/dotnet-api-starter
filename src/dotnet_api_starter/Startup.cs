using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Infrastructure.Repositories;
using dotnet_api_starter.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using dotnet_api_starter.Infrastructure.Services;
using dotnet_api_starter.Resources.User;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace dotnet_api_starter
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);
            ConfigureDI(services);

            JwtSettings jwtSettings = new JwtSettings();
            Configuration.GetSection($"{Environment.EnvironmentName}:Jwt").Bind(jwtSettings);
            AppSettings appSettings = new AppSettings();
            Configuration.GetSection($"{Environment.EnvironmentName}:App").Bind(appSettings);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                    builder
                        .WithOrigins(appSettings.LandingUrl, appSettings.WebUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DictionaryAsArrayResolver());

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),

                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                Debug.WriteLine($"[{context.Connection.RemoteIpAddress}] | {context.Request.GetDisplayUrl()}");
                await next.Invoke();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ConfigureSettings(IServiceCollection services)
        {
            IConfigurationSection environment = Configuration.GetSection(Environment.EnvironmentName);

            services.Configure<DbSettings>(options => { environment.GetSection("MongoConnection").Bind(options); });
            services.Configure<JwtSettings>(options => { environment.GetSection("Jwt").Bind(options); });
            services.Configure<AppSettings>(options => { environment.GetSection("App").Bind(options); });
        }

        private void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserService, UserService>();

        }
    }
}
