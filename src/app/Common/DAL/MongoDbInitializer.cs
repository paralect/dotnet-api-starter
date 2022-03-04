using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Common.DAL.Documents.Token;
using Common.DAL.Documents.User;
using Common.Enums;
using Common.Settings;
using Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Common.DAL
{
    public static class MongoDbInitializer
    {
        public static void InitializeDb(this IServiceCollection services, DbSettings dbSettings)
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("overrides", conventionPack, _ => true);

            // custom serialization/deserialization to store enum Description attributes in DB
            // TODO rewrite to apply to all enums, if possible OR rewrite Node API to store enums as numbers
            BsonSerializer.RegisterSerializer(typeof(TokenType), new EnumSerializer<TokenType>());

            InitializeCollections(services, dbSettings);
        }

        private static void InitializeCollections(IServiceCollection services, DbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            services.AddSingleton<IMongoClient>(client);

            var db = client.GetDatabase(dbSettings.Database);

            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var schemasPath = Path.Combine(assemblyLocation, "DAL", "Schemas");
            var collectionDescriptions = new List<CollectionDescription>
            {
                new()
                {
                    Name = Constants.DbDocuments.Users,
                    DocumentType = typeof(User),
                    SchemaPath = Path.Combine(schemasPath, "UserSchema.json"),
                    IndexDescriptions = new []
                    {
                        new IndexDescription
                        {
                            IndexKeysDefinition = Builders<User>.IndexKeys.Ascending(user => user.Email),
                            Options = new CreateIndexOptions { Unique = true }
                        }
                    }
                },
                new()
                {
                    Name = Constants.DbDocuments.Tokens,
                    DocumentType = typeof(Token),
                    SchemaPath = Path.Combine(schemasPath, "TokenSchema.json")
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

                if (description.IndexDescriptions != null)
                {
                    var indexes = collection?.GetType().GetProperty("Indexes")?.GetValue(collection);
                    if (indexes != null)
                    {
                        var createOneMethodInfo = indexes.GetType().GetMethod(
                            "CreateOne",
                            new[] { typeof(IndexKeysDefinition<>).MakeGenericType(description.DocumentType), typeof(CreateIndexOptions), typeof(CancellationToken) });

                        foreach (var indexDescription in description.IndexDescriptions)
                        {
                            createOneMethodInfo?.Invoke(indexes, new[] { indexDescription.IndexKeysDefinition, indexDescription.Options, default(CancellationToken) });
                        }
                    }
                }

                services.AddSingleton(collectionType, collection);
            }
        }

        private static bool CollectionExists(IMongoDatabase database, string collectionName)
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
        public IList<IndexDescription> IndexDescriptions { get; set; }
    }

    public class IndexDescription
    {
        public object IndexKeysDefinition { get; set; }
        public CreateIndexOptions Options { get; set; }
    }
}
