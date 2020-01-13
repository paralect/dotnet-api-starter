using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DAL;
using Api.Core.DAL.Documents;

namespace Api.Core.Interfaces.DAL
{
    public interface IRepository<TDocument, in TFilter> 
        where TDocument : BaseDocument
        where TFilter : BaseFilter
    {
        Task InsertAsync(TDocument document);
        Task InsertManyAsync(IEnumerable<TDocument> documents);

        Task<TDocument> FindOneAsync(TFilter filter);

        Task UpdateOneAsync(string id, Expression<Func<TDocument, object>> fieldSelector, object value);
        Task UpdateOneAsync(string id, Action<TDocument> updater);

        Task DeleteManyAsync(TFilter filter);
    }
}
