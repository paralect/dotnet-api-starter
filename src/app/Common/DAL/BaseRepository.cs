using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
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

        public async Task UpdateOneAsync(string id, Expression<Func<TDocument, object>> fieldSelector, object value)
        {
            var updateDefinition = Builders<TDocument>.Update.Set(fieldSelector, value);
            await Collection.UpdateOneAsync(GetFilterById(id), updateDefinition);
        }

        public async Task UpdateOneAsync(string id, Action<TDocument> updater)
        {
            using var session = await DbContext.Client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var document = await FindOneAsync(new TFilter { Id = id });
                if (document != null)
                {
                    updater(document);
                    await Collection.ReplaceOneAsync(GetFilterById(id), document);
                }
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }

            await session.CommitTransactionAsync();
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

            return Builders<TDocument>.Filter.And(filterQueries);
        }

        private FilterDefinition<TDocument> GetFilterById(string id)
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
