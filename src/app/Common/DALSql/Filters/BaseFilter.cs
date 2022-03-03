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
            IncludeProperties = new List<Expression<Func<TEntity, object>>>();
        }

        public long? Id { get; set; }
        public List<Expression<Func<TEntity, object>>> IncludeProperties { get; private set; }
        public bool AsNoTracking { get; set; }

        public BaseFilter<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IncludeProperties = includeProperties.ToList();
            return this;
        }

        public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetPredicates();
    }
}