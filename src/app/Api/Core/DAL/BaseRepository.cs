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
    public abstract class BaseRepository<TView, TFilter> : IRepository<TView, TFilter> 
        where TView : BaseView
        where TFilter : BaseFilter
    {
        protected readonly IDbContext DbContext;
        protected readonly IIdGenerator IdGenerator;
        protected readonly IMongoCollection<TView> Collection;

        protected BaseRepository(
            IDbContext dbContext,
            IIdGenerator idGenerator,
            Func<IDbContext, IMongoCollection<TView>> collectionProvider
        )
        {
            DbContext = dbContext;
            IdGenerator = idGenerator;
            Collection = collectionProvider(DbContext);
        }

        public async Task InsertAsync(TView view)
        {
            if (view.Id.HasNoValue())
            {
                view.Id = IdGenerator.Generate();
            }

            await Collection.InsertOneAsync(view);
        }

        public async Task InsertManyAsync(IEnumerable<TView> views)
        {
            var viewsToInsert = views.Select(v =>
            {
                if (v.Id.HasNoValue())
                {
                    v.Id = IdGenerator.Generate();
                }

                return v;
            }).ToList();

            await Collection.InsertManyAsync(viewsToInsert);
        }

        public async Task<TView> FindOneAsync(TFilter filter)
        {
            var result = await Collection.FindAsync(BuildFilterQuery(filter));
            return result.SingleOrDefault();
        }

        public async Task UpdateOneAsync(string id, Expression<Func<TView, object>> fieldSelector, object value)
        {
            await UpdateOneAsync(id, new Dictionary<Expression<Func<TView, object>>, object>
            {
                { fieldSelector, value }
            });
        }

        public async Task UpdateOneAsync(string id, Dictionary<Expression<Func<TView, object>>, object> updates)
        {
            var filter = Builders<TView>.Filter.Eq(x => x.Id, id);
            var builder = Builders<TView>.Update;
            var updateDefinition = builder.Combine(updates.Select(u => builder.Set(u.Key, u.Value)));

            await Collection.UpdateOneAsync(filter, updateDefinition);
        }

        public async Task DeleteManyAsync(TFilter filter)
        {
            await Collection.DeleteManyAsync(BuildFilterQuery(filter));
        }

        protected virtual IEnumerable<FilterDefinition<TView>> GetFilterQueries(TFilter filter)
        {
            return new List<FilterDefinition<TView>>();
        }

        private FilterDefinition<TView> BuildFilterQuery(TFilter filter)
        {
            var filterQueries = GetFilterQueries(filter);
            return Builders<TView>.Filter.And(filterQueries);
        }
    }
}
