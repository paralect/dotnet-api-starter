using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public interface IRepository<TModel>
    {
        Task Insert(TModel model);
        TModel FindOne(Func<TModel, bool> predicate);
        TModel FindById(ObjectId id);
        TModel FindById(string id);
    }
}
