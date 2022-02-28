using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql
{
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

            if (filter.OrderingProperties.Any())
            {
                query = Queryable.OrderBy(query, (dynamic)filter.OrderingProperties[0]);
                if (filter.OrderingProperties.Count > 1)
                {
                    foreach (var orderingProperty in filter.OrderingProperties.Skip(1))
                    {
                        query = Queryable.ThenBy((IOrderedQueryable<TEntity>)query, (dynamic)orderingProperty);
                    }
                }
            }

            if (filter.OrderingByDescendingProperties.Any())
            {
                query = Queryable.OrderByDescending(query, (dynamic)filter.OrderingByDescendingProperties[0]);
                if (filter.OrderingByDescendingProperties.Count > 1)
                {
                    foreach (var orderingProperty in filter.OrderingByDescendingProperties.Skip(1))
                    {
                        query = Queryable.ThenByDescending((IOrderedQueryable<TEntity>)query, (dynamic)orderingProperty);
                    }
                }
            }

            if (filter.SkipCount > 0)
            {
                query = query.Skip(filter.SkipCount);
            }

            if (filter.TakeCount > 0)
            {
                query = query.Take(filter.TakeCount);
            }

            return query;
        }
    }
}