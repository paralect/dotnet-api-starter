using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DALSql.Entities;
using Common.Utils;

namespace Common.DALSql.Filters
{
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