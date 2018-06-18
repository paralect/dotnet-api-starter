using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IRepository<TModel> 
        where TModel : BaseModel
    {
        Task Insert(TModel model);

        TModel FindOne(Func<TModel, bool> predicate);

        TModel FindById(ObjectId id);

        TModel FindById(string id);

        Task<bool> Update(ObjectId id, Expression<Func<TModel, TModel>> updateExpression);
    }
}
