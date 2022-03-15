using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DalSql;
using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.DalSql.Repositories;

public interface IRepository<TEntity, in TFilter>
    where TEntity : BaseEntity
    where TFilter : BaseFilter<TEntity>
{
    Task<TEntity> FindById(long id);
    Task<TEntity> FindOneAsync(TFilter filter);
    Task<Page<TResultModel>> FindPageAsync<TResultModel>(
        TFilter filter,
        ICollection<SortField> sortFields,
        int page,
        int pageSize,
        Expression<Func<TEntity, TResultModel>> map
    );

    Task InsertAsync(TEntity entity);
    Task InsertManyAsync(IEnumerable<TEntity> entities);

    //Task UpdateOneAsync<TField>(string id, Expression<Func<TEntity, TField>> fieldSelector, TField value);
    //Task UpdateOneAsync(string id, IUpdateOperator<TEntity> update);
    //Task UpdateOneAsync(string id, IEnumerable<IUpdateOperator<TEntity>> updates);

    //Task ReplaceOneAsync(string id, Action<TEntity> updater);
    //Task ReplaceOneAsync(TEntity document, Action<TEntity> updater);
    //Task ReplaceOneAsync(TEntity document);

    Task DeleteManyAsync(TFilter filter);
}
