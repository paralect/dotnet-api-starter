using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DalSql;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.ServicesSql.Domain.Interfaces;

namespace Common.ServicesSql.Domain;

public class BaseEntityService<TEntity, TFilter> : IEntityService<TEntity, TFilter>
    where TEntity : BaseEntity
    where TFilter : BaseFilter<TEntity>, new()
{
    private readonly IRepository<TEntity, TFilter> _repository;

    public BaseEntityService(IRepository<TEntity, TFilter> repository)
    {
        _repository = repository;
    }

    public async Task<TEntity> FindByIdAsync(long id)
    {
        return await FindOneAsync(new TFilter { Id = id });
    }

    public async Task<TEntity> FindOneAsync(TFilter filter)
    {
        return await _repository.FindOneAsync(filter);
    }

    public async Task<TResultModel> FindOneAsync<TResultModel>(TFilter filter, Expression<Func<TEntity, TResultModel>> map)
    {
        return await _repository.FindOneAsync(filter, map);
    }

    public async Task<Page<TResultModel>> FindPageAsync<TResultModel>(
        TFilter filter,
        ICollection<SortField> sortFields,
        int page,
        int pageSize,
        Expression<Func<TEntity, TResultModel>> map
    )
    {
        return await _repository.FindPageAsync(filter, sortFields, page, pageSize, map);
    }

    public async Task<bool> AnyAsync(TFilter filter)
    {
        return await _repository.AnyAsync(filter);
    }
}
