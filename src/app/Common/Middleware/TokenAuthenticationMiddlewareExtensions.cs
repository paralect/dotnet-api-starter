using Microsoft.AspNetCore.Builder;

namespace Common.Middleware
{
    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
        
        public static IApplicationBuilder UseTokenAuthenticationSql(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationSqlMiddleware>();
        }
    }
}