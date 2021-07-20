using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.DALSql.Filters
{
    public abstract class BaseFilter<TEntity>
    {
        public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetPredicates();
    }
}