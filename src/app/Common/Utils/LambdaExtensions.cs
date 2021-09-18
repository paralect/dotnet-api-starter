using System;
using System.Linq.Expressions;

namespace Common.Utils
{
    public static class LambdaExtensions
    {
        public static string GetMemberFullName(this LambdaExpression expression)
        {
            if (expression.Body is not MemberExpression)
            {
                throw new ArgumentException("Provided lambda is not MemberExpression type");
            }

            if (expression.Parameters.Count != 1)
            {
                throw new ArgumentException("Invalid MemberExpression");
            }

            return expression.Parameters[0].Type.FullName +
                   expression.Body.ToString()[expression.Parameters[0].Name!.Length..];
        }
    }
}