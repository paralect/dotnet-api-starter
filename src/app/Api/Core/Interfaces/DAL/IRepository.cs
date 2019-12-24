using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DAL;
using Api.Core.DAL.Views;

namespace Api.Core.Interfaces.DAL
{
    public interface IRepository<TView, in TFilter> 
        where TView : BaseView
        where TFilter : BaseFilter
    {
        Task InsertAsync(TView view);
        Task InsertManyAsync(IEnumerable<TView> views);

        Task<TView> FindOneAsync(TFilter filter);

        Task UpdateOneAsync(string id, Expression<Func<TView, object>> fieldSelector, object value);
        Task UpdateOneAsync(string id, Action<TView> updater);

        Task DeleteManyAsync(TFilter filter);
    }
}
