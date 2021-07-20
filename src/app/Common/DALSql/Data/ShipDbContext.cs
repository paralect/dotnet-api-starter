using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.DALSql.Entities;
using Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Data
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
        public static async Task<IEnumerable<TEntity>> FindByFilterAsNoTracking<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
            where TEntity : BaseEntity
            where TFilter : BaseFilter<TEntity>
        {
            var dbQuery = new DbQuery<TEntity>();
            dbQuery.AddFilter(filter);
            
            return await ConstructQuery(table, dbQuery).ToListAsync();
        }
        
        public static async Task<TEntity> FindOneByFilterAsNoTracking<TEntity, TFilter>(this DbSet<TEntity> table, TFilter filter)
            where TEntity : BaseEntity
            where TFilter : BaseFilter<TEntity>
        {
            var dbQuery = new DbQuery<TEntity>();
            dbQuery.AddFilter(filter);
            
            return await ConstructQuery(table, dbQuery).FirstOrDefaultAsync();
        }
        
        public static async Task<TEntity> FindOneAsNoTracking<TEntity>(this DbSet<TEntity> table, long id)
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
        
            return query;
        }
    }

    public abstract class BaseFilter<TEntity>
    {
        public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetPredicates();
    }

    public class UserFilter : BaseFilter<User>
    {
        public string Email { get; set; }
        public string SignupToken { get; set; }
        public string ResetPasswordToken { get; set; }
        public long? IdToExclude { get; set; }

        public override IEnumerable<Expression<Func<User, bool>>> GetPredicates()
        {
            if (Email.HasValue())
            {
                yield return entity => entity.Email == Email;
            }
            
            if (SignupToken.HasValue())
            {
                yield return entity => entity.SignupToken == SignupToken;
            }
            
            if (ResetPasswordToken.HasValue())
            {
                yield return entity => entity.ResetPasswordToken == ResetPasswordToken;
            }

            if (IdToExclude.HasValue)
            {
                yield return entity => entity.Id != IdToExclude;
            }
        }
    }

    public class TokenFilter : BaseFilter<Token>
    {
        public string Value { get; set; }
        public long? UserId { get; set; }

        public override IEnumerable<Expression<Func<Token, bool>>> GetPredicates()
        {
            if (Value.HasValue())
            {
                yield return entity => entity.Value == Value;
            }
            
            if (UserId.HasValue)
            {
                yield return entity => entity.UserId == UserId;
            }
        }
    }
}
