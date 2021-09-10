using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents;
using Common.DB.Mongo.DAL.UpdateDocumentOperators;
using MongoDB.Driver.Linq;

namespace Common.DB.Mongo.DAL.Interfaces
{
    public interface IRepository<TDocument, in TFilter> 
        where TDocument : BaseMongoDocument
        where TFilter : BaseFilter
    {
        Task InsertAsync(TDocument document);
        Task InsertManyAsync(IEnumerable<TDocument> documents);

        Task<TDocument> FindOneAsync(TFilter filter);

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
