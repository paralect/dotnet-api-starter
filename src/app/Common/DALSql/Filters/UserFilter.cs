﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DALSql.Entities;
using Common.Utils;

namespace Common.DALSql.Filters
{
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
}