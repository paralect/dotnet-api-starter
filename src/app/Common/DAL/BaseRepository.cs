using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using Common.DAL.UpdateDocumentOperators;
using Common.Utils;
using MongoDB.Driver;

namespace Common.DAL
{
    public abstract class BaseRepository<TDocument, TFilter> : IRepository<TDocument, TFilter> 
        where TDocument : BaseDocument
        where TFilter : BaseFilter, new()
    {
        protected readonly IDbContext DbContext;
        protected readonly IIdGenerator IdGenerator;
        protected readonly IMongoCollection<TDocument> Collection;

        protected BaseRepository(
            IDbContext dbContext,
            IIdGenerator idGenerator,
            Func<IDbContext, IMongoCollection<TDocument>> collectionProvider
        )
        {
            DbContext = dbContext;
            IdGenerator = idGenerator;
            Collection = collectionProvider(DbContext);
        }

        public async Task InsertAsync(TDocument document)
        {
            AddId(document);

            await Collection.InsertOneAsync(document);
        }

        public async Task InsertManyAsync(IEnumerable<TDocument> documents)
        {
            var documentsToInsert = documents.Select(d =>
            {
                AddId(d);

                return d;
            }).ToList();

            await Collection.InsertManyAsync(documentsToInsert);
        }

        public async Task<TDocument> FindOneAsync(TFilter filter)
        {
            var result = await Collection.FindAsync(BuildFilterQuery(filter));
            return result.SingleOrDefault();
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

        private void AddId(TDocument document)
        {
            if (document.Id.HasNoValue())
            {
                document.Id = IdGenerator.Generate();
            }
        }
    }
}
