﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly object _locker = new object();

        public static void InitializeDb(this IServiceCollection services, DbSettings dbSettings)
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

            lock (_locker)
            {
                InitializeCollections(services, dbSettings);
            }
        }

        private static void InitializeCollections(IServiceCollection services, DbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            services.AddSingleton<IMongoClient>(client);

            var db = client.GetDatabase(dbSettings.Database);

            var schemasPath = "../Common/DAL/Schemas/";
            var collectionDescriptions = new List<CollectionDescription>
            {
                new CollectionDescription
                {
                    Name = Constants.DbDocuments.Users,
                    DocumentType = typeof(User),
                    SchemaPath = $"{schemasPath}UserSchema.json",
                    IndexDescriptions = new []
                    {
                        new IndexDescription
                        {
                            IndexKeysDefinition = Builders<User>.IndexKeys.Ascending(user => user.Email),
                            Options = new CreateIndexOptions { Unique = true }
                        }
                    }
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

                if (description.IndexDescriptions != null)
                {
                    foreach (var indexDescription in description.IndexDescriptions)
                    {
                        var indexes = collection?.GetType().GetProperty("Indexes")?.GetValue(collection);
                        if (indexes == null)
                        {
                            continue;
                        }

                        var createOneMethodInfo = indexes.GetType().GetMethod(
                            "CreateOne", 
                            new [] { typeof(IndexKeysDefinition<>).MakeGenericType(description.DocumentType), typeof(CreateIndexOptions), typeof(CancellationToken) });
                        createOneMethodInfo?.Invoke(indexes, new []{ indexDescription.IndexKeysDefinition, indexDescription.Options, default(CancellationToken) });
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