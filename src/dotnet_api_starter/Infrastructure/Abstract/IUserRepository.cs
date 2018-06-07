using dotnet_api_starter.Resources.User;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public interface IUserRepository : IRepository<UserDocument>
    {
        Task<IEnumerable<UserDocument>> GetAllUsers();
        Task<bool> UpdateField<TField>(ObjectId _id, Expression<Func<UserDocument, TField>> fieldSelector, TField value);
        Task<bool> Replace(UserDocument model);
    }
}
