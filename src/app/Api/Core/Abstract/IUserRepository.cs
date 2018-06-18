using Api.Core.Models.User;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsers();
    }
}
