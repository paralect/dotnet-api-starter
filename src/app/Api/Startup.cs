using Api.Core.Services.Infrastructure;
using Api.Core.Services.Interfaces.Infrastructure;
using Api.Core.Utils;
using Api.Mapping;
using Common.Middleware;
using Common.Services;
using Common.Services.EmailService;
using Common.Services.UserService;
using Common.Settings;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDbSettings = Common.DB.Mongo.Settings.MongoDbSettings;
using PostgresDbSettings = Common.DB.Postgres.Settings.PostgresDbSettings;
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
            ConfigurePostgresDb(services);
            ConfigureMongoDb(services);
            ConfigureSettings(services);
            ConfigureDI(services);

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
            services.Configure<MongoDbSettings>(options => { _configuration.GetSection("MongoConnection").Bind(options); });
            services.Configure<PostgresDbSettings>(options => { _configuration.GetSection("PostgresConnection").Bind(options); });

            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<GoogleSettings>(options => { _configuration.GetSection("Google").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
        }

        private void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGoogleService, GoogleService>();

            services.AddTransient<IAuthService, AuthService>();




        }

        private void ConfigurePostgresDb(IServiceCollection services)
        {
            var dbSettings = new PostgresDbSettings();
            _configuration.GetSection("PostgresConnection").Bind(dbSettings);

            Common.DB.Postgres.DAL.PostgresDbInitializer.InitializeDb(services, dbSettings);

            services.AddLinqToDbContext<Common.DB.Postgres.DAL.Interfaces.IPostgresDbContext, Common.DB.Postgres.DAL.PostgresDbContext>((provider, options) =>
            {
                options
                //will configure the AppDataConnection to use
                //SqlServer with the provided connection string
                //there are methods for each supported database
                .UsePostgreSQL(dbSettings.ConnectionString)

                //default logging will log everything using
                //an ILoggerFactory configured in the provider
                .UseDefaultLogging(provider);
            }, ServiceLifetime.Transient);

            services.AddTransient<Common.DB.Postgres.DAL.Interfaces.IUserRepository, Common.DB.Postgres.DAL.Repositories.UserRepository>();
            services.AddTransient<Common.DB.Postgres.DAL.Interfaces.ITokenRepository, Common.DB.Postgres.DAL.Repositories.TokenRepository>();

            // uncomment to use PostgreSQL DB for authorization 
            //services.AddTransient<IUserService, Common.DB.Postgres.Services.UserService>();
            //services.AddTransient<ITokenService, Common.DB.Postgres.Services.TokenService>();
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
            var dbSettings = new MongoDbSettings();
            _configuration.GetSection("MongoConnection").Bind(dbSettings);

            Common.DB.Mongo.DAL.MongoDbInitializer.InitializeDb(services, dbSettings);

            services.AddTransient<Common.DB.Mongo.DAL.Interfaces.IMongoDbContext, Common.DB.Mongo.DAL.MongoDbContext>();

            services.AddTransient<Common.DB.Mongo.DAL.Interfaces.IUserRepository, Common.DB.Mongo.DAL.Repositories.UserRepository>();
            services.AddTransient<Common.DB.Mongo.DAL.Interfaces.ITokenRepository, Common.DB.Mongo.DAL.Repositories.TokenRepository>();

            // comment if you use PostgreSQL DB for authorization
            services.AddTransient<IUserService, Common.DB.Mongo.Services.UserService>();
            services.AddTransient<ITokenService, Common.DB.Mongo.Services.TokenService>();
        }
    }
}
