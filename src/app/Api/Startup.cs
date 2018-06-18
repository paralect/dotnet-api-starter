using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Core.Abstract;
using Api.DAL.Repositories;
using Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Api.Core.Services;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Api.Settings.Json;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);
            ConfigureDI(services);

            JwtSettings jwtSettings = new JwtSettings();
            _configuration.GetSection("Jwt").Bind(jwtSettings);
            AppSettings appSettings = new AppSettings();
            _configuration.GetSection("App").Bind(appSettings);

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

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new ProducesAttribute("application / json"));
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
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

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<DbSettings>(options => { _configuration.GetSection("MongoConnection").Bind(options); });
            services.Configure<JwtSettings>(options => { _configuration.GetSection("Jwt").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
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
