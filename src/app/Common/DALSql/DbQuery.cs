using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.DALSql.Entities;
using Common.DALSql.Filters;

namespace Common.DALSql
{
    public class DbQuery<TEntity> where TEntity : BaseEntity
    {
        public DbQuery()
        {
            Predicates = new List<Expression<Func<TEntity, bool>>>();
            OrderingProperties = new List<Expression>();
            OrderingByDescendingProperties = new List<Expression>();
            IncludeProperties = new List<Expression<Func<TEntity, object>>>();
        }
        
        public List<Expression<Func<TEntity, bool>>> Predicates { get; }
        public List<Expression> OrderingProperties { get; }
        public List<Expression> OrderingByDescendingProperties { get; }
        public List<Expression<Func<TEntity, object>>> IncludeProperties { get; private set; }
        public int TakeCount { get; private set; }
        public int SkipCount { get; private set; }
        
        public DbQuery<TEntity> AddFilter<TFilter>(TFilter filter) where TFilter : BaseFilter<TEntity>
        {
            var predicates = filter.GetPredicates();
            Predicates.AddRange(predicates);
            return this;
        }
        
        public DbQuery<TEntity> AddOrder<TProperty>(Expression<Func<TEntity, TProperty>> orderingProperty)
        {
            OrderingProperties.Add(orderingProperty);
            return this;
        }
        
        public DbQuery<TEntity> AddOrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> orderingProperty)
        {
            OrderingByDescendingProperties.Add(orderingProperty);
            return this;
        }
        
        public DbQuery<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IncludeProperties = includeProperties.ToList();
            return this;
        }
        
        public DbQuery<TEntity> Take(int count)
        {
            TakeCount = count;
            return this;
        }
        
        public DbQuery<TEntity> Skip(int count)
        {
            SkipCount = count;
            return this;
        }
    }
}