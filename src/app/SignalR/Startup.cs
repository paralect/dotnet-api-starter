using Common.Middleware;
using Common.Services;
using Common.Settings;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalR.Hubs;
using SignalR.Mapping;
using SignalR.Services;
using MongoDbSettings = Common.DB.Mongo.Settings.MongoDbSettings;
using PostgresDbSettings = Common.DB.Postgres.Settings.PostgresDbSettings;

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
            ConfigurePostgresDb(services);
            ConfigureMongoDb(services);
            ConfigureSettings(services);
            ConfigureDI(services);
            ConfigureCors(services);

            services.AddSignalR();
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

        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<MongoDbSettings>(options => { _configuration.GetSection("MongoConnection").Bind(options); });
            services.Configure<PostgresDbSettings>(options => { _configuration.GetSection("PostgresConnection").Bind(options); });

            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<GoogleSettings>(options => { _configuration.GetSection("Google").Bind(options); });
            services.Configure<TokenExpirationSettings>(options => { _configuration.GetSection("TokenExpiration").Bind(options); });
        }

        private void ConfigurePostgresDb(IServiceCollection services)
        {
            var dbSettings = new PostgresDbSettings();
            _configuration.GetSection("PostgresConnection").Bind(dbSettings);

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

            services.AddTransient<Common.DB.Postgres.DAL.Interfaces.ITokenRepository, Common.DB.Postgres.DAL.Repositories.TokenRepository>();

            //services.AddTransient<ITokenService, Common.DB.Postgres.Services.TokenService>();
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
            var dbSettings = new MongoDbSettings();
            _configuration.GetSection("MongoConnection").Bind(dbSettings);

            Common.DB.Mongo.DAL.MongoDbInitializer.InitializeDb(services, dbSettings);

            services.AddTransient<Common.DB.Mongo.DAL.Interfaces.IMongoDbContext, Common.DB.Mongo.DAL.MongoDbContext>();

            services.AddTransient<Common.DB.Mongo.DAL.Interfaces.ITokenRepository, Common.DB.Mongo.DAL.Repositories.TokenRepository>();

            services.AddTransient<ITokenService, Common.DB.Mongo.Services.TokenService>();
            services.AddHostedService<ChangeStreamBackgroundService>();
        }

        private void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IUserHubContext, UserHubContext>();
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
