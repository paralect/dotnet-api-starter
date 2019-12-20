using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DAL.Views;
using Api.Core.Interfaces.DAL;
using Api.Core.Utils;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public abstract class BaseRepository<TModel, TFilter> : IRepository<TModel, TFilter> 
        where TModel : BaseView
        where TFilter : BaseFilter, new()
    {
        protected readonly IDbContext DbContext;
        protected readonly IIdGenerator IdGenerator;
        protected readonly IMongoCollection<TModel> Collection;

        protected BaseRepository(IDbContext dbContext, IIdGenerator idGenerator, Func<IDbContext, IMongoCollection<TModel>> collectionProvider)
        {
            DbContext = dbContext;
            IdGenerator = idGenerator;
            Collection = collectionProvider(DbContext);
        }

        public async Task InsertAsync(TModel model)
        {
            if (model.Id.HasNoValue())
            {
                model.Id = IdGenerator.Generate();
            }

            await Collection.InsertOneAsync(model);
        }

        public async Task InsertManyAsync(IEnumerable<TModel> models)
        {
            var modelsToInsert = models.Select(m =>
            {
                if (m.Id.HasNoValue())
                {
                    m.Id = IdGenerator.Generate();
                }

                return m;
            }).ToList();

            await Collection.InsertManyAsync(modelsToInsert);
        }

        public async Task<TModel> FindOneAsync(TFilter filter)
        {
            var result = await Collection.FindAsync(BuildFilterQuery(filter));
            return result.SingleOrDefault();
        }

        public async Task<TModel> FindByIdAsync(string id)
        {
            var filter = new TFilter { Id = id };
            return await FindOneAsync(filter);
        }

        public async Task UpdateOneAsync(string id, Expression<Func<TModel, object>> fieldSelector, object value)
        {
            await UpdateOneAsync(id, new Dictionary<Expression<Func<TModel, object>>, object>
            {
                { fieldSelector, value }
            });
        }

        public async Task UpdateOneAsync(string id, Dictionary<Expression<Func<TModel, object>>, object> updates)
        {
            var filter = Builders<TModel>.Filter.Eq(x => x.Id, id);
            var builder = Builders<TModel>.Update;
            var updateDefinition =
                builder.Combine(updates.Select(u => builder.Set(u.Key, u.Value)));

            await Collection.UpdateOneAsync(filter, updateDefinition);
        }

        public async Task DeleteManyAsync(TFilter filter)
        {
            await Collection.DeleteManyAsync(BuildFilterQuery(filter));
        }

        protected virtual IEnumerable<FilterDefinition<TModel>> GetFilterQueries(TFilter filter)
        {
            return new List<FilterDefinition<TModel>>();
        }

        private FilterDefinition<TModel> BuildFilterQuery(TFilter filter)
        {
            var filterQueries = GetFilterQueries(filter);
            return Builders<TModel>.Filter.And(filterQueries);
        }
    }
}
