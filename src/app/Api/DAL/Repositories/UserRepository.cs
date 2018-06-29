using Api.Core.Abstract;
using Api.Settings;
using Api.Core.Models.User;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Dal.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDbContext context)
            : base(context)
        { }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return await _collection.AsQueryable().ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }
    }
}
