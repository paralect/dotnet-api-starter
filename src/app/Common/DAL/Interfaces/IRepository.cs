using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.UpdateDocumentOperators;
using MongoDB.Driver.Linq;

namespace Common.DAL.Interfaces
{
    public interface IRepository<TDocument, in TFilter>
        where TDocument : BaseDocument
        where TFilter : BaseFilter
    {
        Task InsertAsync(TDocument document);
        Task InsertManyAsync(IEnumerable<TDocument> documents);

        Task<TDocument> FindOneAsync(TFilter filter);
        Task<Page<TDocument>> FindPageAsync(
            TFilter filter,
            IList<(string, SortDirection)> sortFields,
            int page,
            int pageSize
        );

        Task UpdateOneAsync<TField>(string id, Expression<Func<TDocument, TField>> fieldSelector, TField value);
        Task UpdateOneAsync(string id, IUpdateOperator<TDocument> update);
        Task UpdateOneAsync(string id, IEnumerable<IUpdateOperator<TDocument>> updates);

        Task ReplaceOneAsync(string id, Action<TDocument> updater);
        Task ReplaceOneAsync(TDocument document, Action<TDocument> updater);
        Task ReplaceOneAsync(TDocument document);

        Task DeleteManyAsync(TFilter filter);
        IMongoQueryable<TDocument> GetQueryable();
    }
}
