using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public abstract class BaseRepository<TModel> : IRepository<TModel>
    {
        protected readonly DbContext _context = null;
        protected readonly IMongoCollection<TModel> _collection = null;

        public BaseRepository(IOptions<DbSettings> settings, Func<DbContext, IMongoCollection<TModel>> collectionProvider)
        {
            _context = new DbContext(settings);
            _collection = collectionProvider(_context);
        }

        public async Task Insert(TModel model)
        {
            try
            {
                await _collection.InsertOneAsync(model);
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public TModel FindOne(Func<TModel, bool> predicate)
        {
            try
            {
                return _collection.AsQueryable().Where(predicate).SingleOrDefault();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public TModel FindById(ObjectId id)
        {
            try
            {
                var filter = Builders<TModel>.Filter.Eq("_id", id);
                return _collection.Find(filter).SingleOrDefault();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public TModel FindById(string id)
        {
            return FindById(ObjectId.Parse(id));
        }
    }
}
