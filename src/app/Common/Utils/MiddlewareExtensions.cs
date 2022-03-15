using Microsoft.AspNetCore.Builder;

namespace Common.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenAuthenticationMiddleware>();
    }

    public static IApplicationBuilder UseTokenAuthenticationSql(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenAuthenticationMiddlewareSql>();
    }

    public static IApplicationBuilder UseDbContextSaveChanges(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DbContextSaveChangesMiddleware>();
    }
}
