using System;
using Common.DAL.Documents.Token;

namespace Common.Utils
{
    public static class DomainExtensions
    {
        public static bool IsExpired(this Token token)
        {
            return token.ExpireAt <= DateTime.UtcNow;
        }
        
        public static bool IsExpired(this DALSql.Entities.Token token)
        {
            return token.ExpireAt <= DateTime.UtcNow;
        }
    }
}