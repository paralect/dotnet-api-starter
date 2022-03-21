using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DalSql;
using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.ServicesSql.Domain.Interfaces;

public interface IEntityService<TEntity, in TFilter>
    where TEntity : BaseEntity
    where TFilter : BaseFilter<TEntity>
{
    Task<TEntity> FindByIdAsync(long id);
    Task<TEntity> FindOneAsync(TFilter filter);
    Task<Page<TResultModel>> FindPageAsync<TResultModel>(
        TFilter filter,
        ICollection<SortField> sortFields,
        int page,
        int pageSize,
        Expression<Func<TEntity, TResultModel>> map
    );
}
