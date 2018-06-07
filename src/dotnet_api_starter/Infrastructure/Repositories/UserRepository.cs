using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Infrastructure.Settings;
using dotnet_api_starter.Resources.User;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<UserDocument>, IUserRepository
    {
        public UserRepository(IOptions<DbSettings> settings)
            : base(settings, context => context.Users)
        { }

        public async Task<IEnumerable<UserDocument>> GetAllUsers()
        {
            try
            {
                return await _context.Users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public async Task<bool> UpdateField<TField>(ObjectId _id, Expression<Func<UserDocument, TField>> fieldSelector, TField value)
        {
            var filter = Builders<UserDocument>.Filter.Eq(s => s._id, _id);
            var update = Builders<UserDocument>.Update.Set(fieldSelector, value);

            try
            {
                UpdateResult actionResult = await _context.Users.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public async Task<bool> Replace(UserDocument model)
        {
            var filter = Builders<UserDocument>.Filter.Eq(x => x._id, model._id);

            try
            {
                ReplaceOneResult actionResult = await _context.Users.ReplaceOneAsync(filter, model);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        
    }
}
