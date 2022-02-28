using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.DALSql.Entities;

namespace Common.DALSql.Filters
{
    public abstract class BaseFilter<TEntity> where TEntity : BaseEntity
    {
        protected BaseFilter()
        {
            OrderingProperties = new List<Expression>();
            OrderingByDescendingProperties = new List<Expression>();
            IncludeProperties = new List<Expression<Func<TEntity, object>>>();
        }

        public long? Id { get; set; }
        public List<Expression> OrderingProperties { get; }
        public List<Expression> OrderingByDescendingProperties { get; }
        public List<Expression<Func<TEntity, object>>> IncludeProperties { get; private set; }
        public int TakeCount { get; private set; }
        public int SkipCount { get; private set; }
        public bool AsNoTracking { get; set; }

        public BaseFilter<TEntity> AddOrder<TProperty>(Expression<Func<TEntity, TProperty>> orderingProperty)
        {
            OrderingProperties.Add(orderingProperty);
            return this;
        }

        public BaseFilter<TEntity> AddOrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> orderingProperty)
        {
            OrderingByDescendingProperties.Add(orderingProperty);
            return this;
        }

        public BaseFilter<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IncludeProperties = includeProperties.ToList();
            return this;
        }

        public BaseFilter<TEntity> Take(int count)
        {
            TakeCount = count;
            return this;
        }

        public BaseFilter<TEntity> Skip(int count)
        {
            SkipCount = count;
            return this;
        }

        public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetPredicates();
    }
}