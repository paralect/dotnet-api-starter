using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Common.DalSql.Repositories;

public abstract class BaseRepository<TEntity, TFilter> : IRepository<TEntity, TFilter>
    where TEntity : BaseEntity
    where TFilter : BaseFilter<TEntity>, new()
{
    protected readonly ShipDbContext dbContext;
    protected readonly DbSet<TEntity> table;

    protected BaseRepository(
        ShipDbContext dbContext,
        Func<ShipDbContext, DbSet<TEntity>> tableProvider
    )
    {
        this.dbContext = dbContext;
        table = tableProvider(this.dbContext);
    }

    public async Task<TEntity> FindById(long id)
    {
        return await FindOneAsync(new TFilter
        {
            Id = id
        });
    }

    public async Task<TEntity> FindOneAsync(TFilter filter)
    {
        return await ConstructQuery(filter).FirstOrDefaultAsync();
    }

    public async Task<Page<TResultModel>> FindPageAsync<TResultModel>(
        TFilter filter,
        ICollection<SortField> sort,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TResultModel>> map)
    {
        var query = ConstructQuery(filter);
        var count = await query.CountAsync();

        IEnumerable<TResultModel> items;

        var skip = (pageNumber - 1) * pageSize;
        if (skip >= count)
        {
            items = Enumerable.Empty<TResultModel>();
        }
        else
        {
            items = await query
                .Order(sort)
                .Skip(skip)
                .Take(pageSize)
                .Select(map)
                .ToListAsync();
        }

        return new Page<TResultModel>
        {
            TotalPages = (int)Math.Ceiling((float)count / pageSize),
            Count = count,
            Items = items
        };
    }

    public async Task InsertAsync(TEntity entity)
    {
        table.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities)
    {
        table.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateOneAsync(TEntity entity)
    {
        table.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteManyAsync(TFilter filter)
    {
        var entities = ConstructQuery(filter);
        table.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> ConstructQuery(BaseFilter<TEntity> filter)
    {
        var query = table.AsQueryable();
        if (filter.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        var predicates = filter.GetPredicates().ToList();
        if (filter.Id.HasValue)
        {
            predicates.Add(entity => entity.Id == filter.Id);
        }

        if (predicates.Any())
        {
            predicates.ForEach(predicate => query = query.Where(predicate));
        }

        if (filter.IncludeProperties.Any())
        {
            foreach (var includeProperty in filter.IncludeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return query;
    }
}
