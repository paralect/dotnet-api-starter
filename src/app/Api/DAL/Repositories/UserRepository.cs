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
        public UserRepository(IOptions<DbSettings> settings)
            : base(settings, dbContext => dbContext.Users)
        { }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return await _context.Users.AsQueryable().ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }
    }
}
