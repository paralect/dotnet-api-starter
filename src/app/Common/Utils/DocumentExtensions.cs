using System;
using Common.Models;

namespace Common.Utils
{
    public static class DocumentExtensions
    {
        public static bool IsExpired(this IToken token)
        {
            return token.ExpireAt <= DateTime.UtcNow;
        }
    }
}