using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DbViews;

namespace Api.Core.Interfaces.DAL
{
    public interface IRepository<TModel> 
        where TModel : BaseView
    {
        Task InsertAsync(TModel model);

        Task InsertManyAsync(IEnumerable<TModel> models);

        TModel FindOne(Func<TModel, bool> predicate);

        TModel FindById(string id);

        Task<bool> UpdateAsync(string id, Expression<Func<TModel, TModel>> updateExpression);

        Task DeleteManyAsync(Expression<Func<TModel, bool>> deleteExpression);
    }
}
