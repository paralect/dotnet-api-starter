using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents;
using Common.DB.Mongo.DAL.UpdateDocumentOperators;

namespace Common.DB.Mongo.DAL.Interfaces
{
    public interface IRepository<TDocument, in TFilter> 
        where TDocument : BaseMongoDocument
        where TFilter : BaseFilter
    {
        Task InsertAsync(TDocument document);
        Task InsertManyAsync(IEnumerable<TDocument> documents);

        Task<TDocument> FindOneAsync(TFilter filter);

        Task UpdateOneAsync<TField>(Guid id, Expression<Func<TDocument, TField>> fieldSelector, TField value);
        Task UpdateOneAsync(Guid id, IUpdateOperator<TDocument> update);
        Task UpdateOneAsync(Guid id, IEnumerable<IUpdateOperator<TDocument>> updates);

        Task ReplaceOneAsync(Guid id, Action<TDocument> updater);
        Task ReplaceOneAsync(TDocument document, Action<TDocument> updater);
        Task ReplaceOneAsync(TDocument document);

        Task DeleteManyAsync(TFilter filter);
    }
}
