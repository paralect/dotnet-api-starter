using System;
using Common.DAL.Documents;

namespace Common.Utils
{
    public static class DocumentExtensions
    {
        public static bool IsExpired(this Token token)
        {
            return token.ExpireAt <= DateTime.UtcNow;
        }
    }
}