using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DALSql.Data;
using Common.DALSql.Entities;

namespace Common.DALSql
{
    public class DbQuery<TEntity> where TEntity : BaseEntity
    {
        public DbQuery()
        {
            Predicates = new List<Expression<Func<TEntity, bool>>>();
        }
        
        public List<Expression<Func<TEntity, bool>>> Predicates { get; set; }
        
        public DbQuery<TEntity> AddFilter(Expression<Func<TEntity, bool>> predicate)
        {
            Predicates.Add(predicate);
            return this;
        }

        public DbQuery<TEntity> AddFilter<TFilter>(TFilter filter) where TFilter : BaseFilter<TEntity>
        {
            var predicates = filter.GetPredicates();
            Predicates.AddRange(predicates);
            return this;
        }
    }
}