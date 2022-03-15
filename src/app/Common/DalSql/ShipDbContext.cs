using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Common.DalSql;

public class ShipDbContext : DbContext
{
    public ShipDbContext(DbContextOptions<ShipDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
}

public static class DbSetExtensions
{
    public static async Task<IEnumerable<TEntity>> FindByFilterAsync<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
        where TEntity : BaseEntity
        where TFilter : BaseFilter<TEntity>
    {
        return await ConstructQuery(table, filter).ToListAsync();
    }

    public static async Task<TEntity> FindOneByFilterAsync<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
        where TEntity : BaseEntity
        where TFilter : BaseFilter<TEntity>
    {
        return await ConstructQuery(table, filter).FirstOrDefaultAsync();
    }

    public static async Task<Page<TResultModel>> FindPageAsync<TEntity, TFilter, TResultModel>(
        this DbSet<TEntity> table,
        TFilter filter,
        IList<SortField> sort,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TResultModel>> map)
        where TEntity : BaseEntity
        where TFilter : BaseFilter<TEntity>
    {
        var query = ConstructQuery(table, filter);
        var count = await query.CountAsync();

        var skip = (pageNumber - 1) * pageSize;
        if (skip >= count)
        {
            return new Page<TResultModel>
            {
                Count = count,
                Items = Enumerable.Empty<TResultModel>()
            };
        }

        var result = await query
            .Order(sort)
            .Skip(skip)
            .Take(pageSize)
            .Select(map)
            .ToListAsync();

        return new Page<TResultModel>
        {
            TotalPages = (int)Math.Ceiling((float)count / pageSize),
            Count = count,
            Items = result
        };
    }

    private static IQueryable<TEntity> ConstructQuery<TEntity>(DbSet<TEntity> table, BaseFilter<TEntity> filter)
        where TEntity : BaseEntity
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
