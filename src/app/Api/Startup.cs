using System;
using System.Collections.Generic;
using System.IO;
using Api.Core;
using Api.Core.DAL;
using Api.Core.DAL.Repositories;
using Api.Core.DAL.Views.Token;
using Api.Core.DAL.Views.User;
using Api.Core.Enums;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Services.App;
using Api.Core.Services.Infrastructure;
using Api.Core.Settings;
using Api.Core.Utils;
using Api.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using IIdGenerator = Api.Core.Interfaces.DAL.IIdGenerator;
using ValidationAttribute = Api.Security.ValidationAttribute;

namespace Api
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
                        .AllowAnyMethod();
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
            services.Configure<DbSettings>(options => { _configuration.GetSection("MongoConnection").Bind(options); });
            services.Configure<AppSettings>(options => { _configuration.GetSection("App").Bind(options); });
            services.Configure<GoogleSettings>(options => { _configuration.GetSection("Google").Bind(options); });
        }

        private void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IGoogleService, GoogleService>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();

            services.AddTransient<IDbContext, DbContext>();

            services.AddTransient<IIdGenerator, IdGenerator>();
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("overrides", conventionPack, t => true);

            // custom serialization/deserialization to store enum Description attributes in DB
            // TODO rewrite to apply to all enums, if possible OR rewrite Node API to store enums as numbers
            BsonSerializer.RegisterSerializer(typeof(TokenTypeEnum), new EnumSerializer<TokenTypeEnum>());

            InitializeCollections(services);
        }

        private void InitializeCollections(IServiceCollection services)
        {
            var dbSettings = new DbSettings();
            _configuration.GetSection("MongoConnection").Bind(dbSettings);

            var client = new MongoClient(dbSettings.ConnectionString);
            var db = client.GetDatabase(dbSettings.Database);

            var schemasPath = "Core/DAL/Schemas/";
            var collectionDescriptions = new List<CollectionDescription>
            {
                new CollectionDescription
                {
                    Name = Constants.DbDocuments.Users,
                    DocumentType = typeof(User),
                    SchemaPath = $"{schemasPath}UserSchema.json"
                },
                new CollectionDescription
                {
                    Name = Constants.DbDocuments.Tokens,
                    DocumentType = typeof(Token),
                    SchemaPath = $"{schemasPath}TokenSchema.json"
                }
            };

            foreach (var description in collectionDescriptions)
            {
                // the call to CreateCollection is necessary to apply validation schema to the collection
                if (!CollectionExists(db, description.Name))
                {
                    var createCollectionOptionsType = typeof(CreateCollectionOptions<>).MakeGenericType(description.DocumentType);
                    dynamic createCollectionOptions = Activator.CreateInstance(createCollectionOptionsType);

                    if (description.SchemaPath.HasValue())
                    {
                        var schema = File.ReadAllText(description.SchemaPath);
                        createCollectionOptions.Validator = BsonDocument.Parse(schema);
                    }

                    db.CreateCollection(description.Name, createCollectionOptions);
                }

                var method = typeof(IMongoDatabase).GetMethod("GetCollection");
                var generic = method.MakeGenericMethod(description.DocumentType);
                var collection = generic.Invoke(db, new object[] { description.Name, null });
                var collectionType = typeof(IMongoCollection<>).MakeGenericType(description.DocumentType);

                services.AddSingleton(collectionType, collection);
            }
        }

        private bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };

            return database.ListCollectionNames(options).Any();
        }
    }

    public class CollectionDescription
    {
        public string Name { get; set; }
        public Type DocumentType { get; set; }
        public string SchemaPath { get; set; }
    }
}
