using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents;
using Common.DB.Mongo.DAL.Interfaces;
using Common.DB.Mongo.DAL.UpdateDocumentOperators;
using Common.Utils;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Common.DB.Mongo.DAL
{
    public abstract class BaseRepository<TDocument, TFilter> : IRepository<TDocument, TFilter>
        where TDocument : BaseMongoDocument
        where TFilter : BaseFilter, new()
    {
        protected readonly IMongoDbContext DbContext;
        protected readonly IMongoCollection<TDocument> Collection;

        protected BaseRepository(
            IMongoDbContext dbContext,
            Func<IMongoDbContext, IMongoCollection<TDocument>> collectionProvider
        )
        {
            DbContext = dbContext;
            Collection = collectionProvider(DbContext);
        }

        public Task InsertAsync(TDocument document)
        {
            return Collection.InsertOneAsync(document);
        }

        public Task InsertManyAsync(IEnumerable<TDocument> documents)
        {
            return Collection.InsertManyAsync(documents);
        }

        public async Task<TDocument> FindOneAsync(TFilter filter)
        {
            var result = await Collection.FindAsync(BuildFilterQuery(filter));
            return result.SingleOrDefault();
        }

        public IMongoQueryable<TDocument> GetQueryable()
        {
            return Collection.AsQueryable();
        }

        public async Task UpdateOneAsync<TField>(string id, Expression<Func<TDocument, TField>> fieldSelector, TField value)
        {
            await UpdateOneAsync(id, new SetOperator<TDocument, TField>(fieldSelector, value));
        }

        public async Task UpdateOneAsync(string id, IUpdateOperator<TDocument> update)
        {
            await UpdateOneAsync(id, new[] { update });
        }

        public async Task UpdateOneAsync(string id, IEnumerable<IUpdateOperator<TDocument>> updates)
        {
            var filterDefinition = GetFilterById(id);
            var updateDefinition = Builders<TDocument>.Update.Combine(updates.Select(update => update.ToUpdateDefinition()));

            await Collection.UpdateOneAsync(filterDefinition, updateDefinition);
        }

        public async Task ReplaceOneAsync(TDocument document)
        {
            await Collection.ReplaceOneAsync(GetFilterById(document.Id), document);
        }

        public async Task ReplaceOneAsync(TDocument document, Action<TDocument> updater)
        {
            updater(document);
            await ReplaceOneAsync(document);
        }

        public async Task ReplaceOneAsync(string id, Action<TDocument> updater)
        {
            var document = await FindOneAsync(new TFilter { Id = id });
            await ReplaceOneAsync(document, updater);
        }

        public async Task DeleteManyAsync(TFilter filter)
        {
            await Collection.DeleteManyAsync(BuildFilterQuery(filter));
        }

        protected virtual IEnumerable<FilterDefinition<TDocument>> GetFilterQueries(TFilter filter)
        {
            return new List<FilterDefinition<TDocument>>();
        }

        private FilterDefinition<TDocument> BuildFilterQuery(TFilter filter)
        {
            var filterQueries = GetFilterQueries(filter).ToList();
            if (filter.Id.HasValue())
            {
                filterQueries.Add(GetFilterById(filter.Id));
            }

            if (!filterQueries.Any() && !filter.IsEmptyFilterAllowed)
            {
                throw new ApplicationException("Empty filter is not allowed");
            }

            return filterQueries.Any()
                ? Builders<TDocument>.Filter.And(filterQueries)
                : FilterDefinition<TDocument>.Empty;
        }

        private static FilterDefinition<TDocument> GetFilterById(string id)
        {
            return Builders<TDocument>.Filter.Eq(d => d.Id, id);
        }
    }
}
