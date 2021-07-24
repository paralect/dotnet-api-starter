using Api.Core.Services.Domain;
using Api.Core.Services.Infrastructure;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Api.Core.Settings;
using Api.Core.Utils;
using Api.Mapping;
using Common.DAL;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.DALSql;
using Common.Middleware;
using Common.Services;
using Common.Services.Interfaces;
using Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using IIdGenerator = Common.DAL.Interfaces.IIdGenerator;
using ValidationAttribute = Api.Security.ValidationAttribute;

namespace Api
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
            ConfigureDI(services);
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

            // choose one of these options depending on where tokens are stored
            app.UseTokenAuthentication();
            // app.UseTokenAuthenticationSql();
            
            // Only for SQL DB. The middleware makes requests to DB,
            // if there are any changes on EF DbContext.
            // It's still possible to update DB manually from controllers/services -
            // in this case the middleware does nothing.
            // app.UseDbContextSaveChanges();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<DbSettings>(options => { _configuration.GetSection("MongoConnection").Bind(options); });
            services.Configure<SqlConnectionSettings>(options => { _configuration.GetSection("SqlConnection").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<GoogleSettings>(options => { _configuration.GetSection("Google").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
        }

        private void ConfigureDI(IServiceCollection services)
        {
            // MongoDB
            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IGoogleService, GoogleService>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();

            services.AddTransient<IDbContext, DbContext>();

            services.AddTransient<IIdGenerator, IdGenerator>();
            
            // PostgreSQL
            
            services.AddTransient<IUserSqlService, UserSqlService>();
            services.AddTransient<IAuthSqlService, AuthSqlService>();
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var dbSettings = new DbSettings();
            _configuration.GetSection("MongoConnection").Bind(dbSettings);

            services.InitializeDb(dbSettings);
            
            var sqlConnectionSettings = new SqlConnectionSettings();
            _configuration.GetSection("SqlConnection").Bind(sqlConnectionSettings);
            
            // uncomment to use PostgreSQL DB
            // services.InitializePostgreSqlDb(sqlConnectionSettings);
        }
    }
}
