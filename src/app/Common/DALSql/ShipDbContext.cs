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
        public static async Task<IEnumerable<TEntity>> FindByFilterAsNoTrackingAsync<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
            where TEntity : BaseEntity
            where TFilter : BaseFilter<TEntity>
        {
            var dbQuery = new DbQuery<TEntity>();
            dbQuery.AddFilter(filter);
            
            return await ConstructQuery(table, dbQuery).ToListAsync();
        }
        
        public static async Task<TEntity> FindOneByFilterAsNoTrackingAsync<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
            where TEntity : BaseEntity
            where TFilter : BaseFilter<TEntity>
        {
            var dbQuery = new DbQuery<TEntity>();
            dbQuery.AddFilter(filter);
            
            return await ConstructQuery(table, dbQuery).FirstOrDefaultAsync();
        }
        
        public static async Task<TEntity> FindOneAsNoTrackingAsync<TEntity>(this DbSet<TEntity> table, long id)
            where TEntity : BaseEntity
        {
            return await table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        
        private static IQueryable<TEntity> ConstructQuery<TEntity>(DbSet<TEntity> table, DbQuery<TEntity> queryParams)
            where TEntity : BaseEntity
        {
            var query = table.AsNoTracking();
            if (queryParams.Predicates.Any())
            {
                queryParams.Predicates.ForEach(predicate => query = query.Where(predicate));
            }
            
            if (queryParams.IncludeProperties.Any())
            {
                foreach (var includeProperty in queryParams.IncludeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            
            if (queryParams.OrderingProperties.Any())
            {
                query = Queryable.OrderBy(query, (dynamic)queryParams.OrderingProperties[0]);
                if (queryParams.OrderingProperties.Count > 1)
                {
                    foreach (var orderingProperty in queryParams.OrderingProperties.Skip(1))
                    {
                        query = Queryable.ThenBy((IOrderedQueryable<TEntity>)query, (dynamic)orderingProperty);
                    }
                }
            }
            
            if (queryParams.OrderingByDescendingProperties.Any())
            {
                query = Queryable.OrderByDescending(query, (dynamic)queryParams.OrderingByDescendingProperties[0]);
                if (queryParams.OrderingByDescendingProperties.Count > 1)
                {
                    foreach (var orderingProperty in queryParams.OrderingByDescendingProperties.Skip(1))
                    {
                        query = Queryable.ThenByDescending((IOrderedQueryable<TEntity>)query, (dynamic)orderingProperty);
                    }
                }
            }
            
            if (queryParams.SkipCount > 0)
            {
                query = query.Skip(queryParams.SkipCount);
            }
            
            if (queryParams.TakeCount > 0)
            {
                query = query.Take(queryParams.TakeCount);
            }
            
            return query;
        }
    }
}
