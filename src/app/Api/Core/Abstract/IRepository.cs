using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IRepository<TModel> 
        where TModel : BaseModel
    {
        Task Insert(TModel model);

        TModel FindOne(Func<TModel, bool> predicate);

        TModel FindById(string id);

        Task<bool> Update(string id, Expression<Func<TModel, TModel>> updateExpression);
    }
}
