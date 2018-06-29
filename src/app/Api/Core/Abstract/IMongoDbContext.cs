using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IMongoDbContext
    {
        IMongoCollection<TModel> GetCollection<TModel>();
    }
}
