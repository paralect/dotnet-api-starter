﻿using System.Threading;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents.User;
using Common.DB.Mongo.DAL.Interfaces;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using SignalR.Hubs;

namespace SignalR.Services
{
    public class ChangeStreamBackgroundService : BackgroundService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IUserHubContext _userHubContext;

        public ChangeStreamBackgroundService(IMongoDbContext dbContext, IUserHubContext userHubContext)
        {
            _users = dbContext.Users;
            _userHubContext = userHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<User>>()
                .Match(x => x.OperationType == ChangeStreamOperationType.Replace || x.OperationType == ChangeStreamOperationType.Update);

            using var cursor = _users.Watch(pipeline, options, stoppingToken);
            await cursor.ForEachAsync(async document =>
            {
                await _userHubContext.SendUpdateAsync(document.FullDocument);
            }, stoppingToken);
        }
    }
}
