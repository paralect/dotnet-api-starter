using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DALSql.Entities;

namespace Common.DALSql
{
    public class DbQuery<T> where T : BaseEntity
    {
        public DbQuery()
        {
            Predicates = new List<Expression<Func<T, bool>>>();
        }
        
        public List<Expression<Func<T, bool>>> Predicates { get; set; }
        
        public  DbQuery<T> AddFilter(Expression<Func<T, bool>> predicate)
        {
            Predicates.Add(predicate);
            return this;
        }
    }
}