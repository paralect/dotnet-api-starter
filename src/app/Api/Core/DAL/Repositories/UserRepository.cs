using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Core.DAL.Repositories
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
